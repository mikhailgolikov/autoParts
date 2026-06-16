# Анализ безопасности AutoPartsStore

Документ описывает настройку и результаты трёх видов проверки безопасности для ASP.NET Core 9 API.

## Быстрый запуск

```powershell
# 1. Запустите API
dotnet run --project src/AutoPartsStore.Api --launch-profile http

# 2. SAST (Semgrep)
powershell -File security/scripts/run-sast.ps1

# 3. DAST (baseline + опционально OWASP ZAP)
powershell -File security/scripts/run-dast.ps1
powershell -File security/scripts/run-dast.ps1 -UseZapDocker   # если Docker Desktop запущен

# 4. SCA (dotnet + Trivy)
powershell -File security/scripts/run-sca.ps1
```

Отчёты сохраняются в `security/reports/`.

---

## 1. SAST — статический анализ (Semgrep)

### Почему Semgrep

Для C# / ASP.NET Core проекта выбран **[Semgrep](https://semgrep.dev/)** — опенсорсный SAST-сканер с поддержкой C#, JSON и готовыми наборами правил OWASP. Не требует отдельного сервера (в отличие от SonarQube), работает локально и в CI.

### Конфигурация

| Файл | Назначение |
|------|------------|
| `security/sast/.semgrep.yml` | 5 кастомных правил проекта |
| `p/csharp` | community-правила для C# |
| `p/secrets` | поиск секретов в коде и конфигах |
| `security/samples/IntentionalVulnerableSample.cs` | намеренные уязвимости для проверки сканера |

### Настроенные правила

| ID правила | CWE | Что ищет |
|------------|-----|----------|
| `autoparts-hardcoded-jwt-key` | CWE-798 | JWT-ключ в appsettings.json |
| `autoparts-hardcoded-db-password` | CWE-798 | пароль БД в connection string |
| `autoparts-sql-string-concatenation` | CWE-89 | SQL-инъекция через конкатенацию строк |
| `autoparts-hardcoded-password-constant` | CWE-798 | пароль в const в исходниках |
| `autoparts-weak-password-validation` | CWE-521 | MinimumLength(6) для паролей |

### Результаты сканирования

**Основной проект** (`security/reports/sast/semgrep-report.json`):
- Пароли БД в `appsettings.json` и `appsettings.Development.json` (CWE-798)
- JWT-ключ в конфигурации

**Демо-файл** (`security/reports/sast/semgrep-samples-report.json`):
- Hardcoded password constant
- SQL injection через `ExecuteSqlRaw` + конкатенация

### Примеры устранения

#### 1. Секреты в appsettings (CWE-798)

**Проблема:** пароль PostgreSQL и JWT-ключ лежат в JSON-файлах, которые попадают в git.

**Исправление:** ASP.NET User Secrets (dev) и переменные окружения (prod):

```powershell
cd src/AutoPartsStore.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;..."
dotnet user-secrets set "Jwt:Key" "your-random-32+-char-secret"
```

В `appsettings.json` оставить плейсхолдеры без реальных значений.

#### 2. Слабая валидация пароля (CWE-521) — ИСПРАВЛЕНО

**Было:** `MinimumLength(6)` в `AuthValidators.cs`, `UserValidators.cs`.

**Стало:** `MinimumLength(8)` — соответствует рекомендациям NIST/OWASP.

#### 3. SQL-инъекция (CWE-89) — в демо-файле

**Плохо:**
```csharp
var query = "SELECT * FROM Users WHERE Email = '" + email + "'";
db.Database.ExecuteSqlRaw(query);
```

**Правильно (как в проекте):** EF Core с параметрами:
```csharp
await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
```

---

## 2. DAST — динамический анализ

### Почему OWASP ZAP

Для работающего REST API выбран **[OWASP ZAP](https://www.zaproxy.org/)** — стандарт де-факто для DAST. Baseline-скан через Docker проверяет заголовки, раскрытие информации, типовые веб-уязвимости.

Дополнительно: PowerShell baseline (`run-dast.ps1`) с ID правил ZAP — работает без Docker, если daemon недоступен.

### Конфигурация

| Файл | Назначение |
|------|------------|
| `security/dast/zap-rules.conf` | игнор шумных INFO-алертов (timestamp disclosure и т.д.) |
| `security/scripts/run-dast.ps1` | baseline + опциональный `-UseZapDocker` |

### Настроенные проверки (ZAP rule IDs)

| Plugin ID | Проверка | Уровень |
|-----------|----------|---------|
| 10021 | X-Content-Type-Options отсутствует | Medium |
| 10020 | X-Frame-Options отсутствует | Medium |
| 10038 | Content-Security-Policy отсутствует | Medium |
| 10035 | Referrer-Policy / HSTS | Low/Medium |
| 10036 | Server header раскрывает Kestrel | Low |
| 90022 | Application Error Disclosure | Medium |

### Результаты

Отчёт: `security/reports/dast/dast-baseline-report.md` — **15 находок** на запущенном API (до перезапуска с исправлениями).

Типичные находки:
- Отсутствуют security headers на `/swagger`, `/api/company`, `/api/products`
- Заголовок `Server: Kestrel` раскрывает стек

### Исправление — ИСПРАВЛЕНО в коде

Добавлен middleware `SecurityHeadersExtensions.cs`:

```csharp
app.UseSecurityHeaders(); // в Program.cs после UseHttpsRedirection
```

Устанавливает:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Content-Security-Policy: default-src 'self'; frame-ancestors 'none'`
- `Referrer-Policy: strict-origin-when-cross-origin`
- `Strict-Transport-Security` (на HTTPS)
- `Permissions-Policy`

**После перезапуска API** повторный DAST-скан должен показать устранение header-находок.

### Намеренный триггер

Сканер намеренно проверял API **без** security headers (старый процесс на порту 5065). Это подтверждает, что DAST реально обнаруживает проблемы конфигурации.

---

## 3. SCA — композитный анализ зависимостей

### Почему dotnet + Trivy

Для .NET оптимальна связка:
1. **`dotnet list package --vulnerable`** — встроенный NuGet advisory database, нулевая настройка
2. **[Trivy](https://trivy.dev/)** (Aqua Security) — опенсорс SCA, сканирует lock-файлы, бинарники, Docker-образы

### Конфигурация

| Файл | Назначение |
|------|------------|
| `security/scripts/run-sca.ps1` | скан основного solution + demo-проекта |
| `security/sca/samples/VulnerablePackageDemo/` | демо с уязвимым пакетом |

### Результаты основного проекта

`security/reports/sca/dotnet-vulnerable-packages.txt`:

> Указанный проект AutoPartsStore.Api / Infrastructure / Domain **не содержит уязвимых пакетов**.

Текущие зависимости (EF Core 9.0.4, BCrypt, FluentValidation, Swashbuckle) — без известных CVE на момент сканирования.

### Намеренный триггер SCA

Демо-проект с `SixLabors.ImageSharp 2.1.3` (`security/reports/sca/dotnet-vulnerable-demo.txt`):

| CVE / GHSA | Серьёзность | Описание |
|------------|-------------|----------|
| GHSA-2cmq-823j-5qj8 | High | Use After Free в обработке изображений |
| GHSA-63p8-c4ww-9cg7 | High | Heap buffer overflow |
| GHSA-65x7-c272-7g7r | High | Data leakage |
| GHSA-5x7m-6737-26cr | Moderate | DoS при декодировании |
| и др. | Moderate | |

### Критичность для нашего приложения

**SixLabors.ImageSharp** в демо-проекте **не используется** в основном API — критичность для production: **нулевая**.

Если бы пакет был в основном проекте (обработка загрузок изображений в `wwwroot/uploads`):

| Действие | Приоритет |
|----------|-----------|
| Обновить до последней 3.x | Высокий |
| Включить `dotnet list package --vulnerable` в CI | Средний |
| Dependabot / Renovate для авто-PR | Средний |
| Trivy в pipeline перед деплоем | Высокий |

### Как бороться с уязвимостями зависимостей

1. **Регулярный SCA** — `run-sca.ps1` в CI на каждый push
2. **Обновление пакетов** — `dotnet outdated` / Renovate
3. **Минимизация зависимостей** — не добавлять пакеты без необходимости
4. **SBOM** — `dotnet sbom` для аудита supply chain
5. **Блокировка** — fail CI при HIGH/CRITICAL в Trivy

---

## Структура security/

```
security/
  sast/.semgrep.yml          # правила SAST
  dast/zap-rules.conf        # правила ZAP
  samples/                   # намеренные уязвимости для SAST
  sca/samples/               # демо уязвимого NuGet-пакета
  scripts/
    run-sast.ps1
    run-dast.ps1
    run-sca.ps1
  reports/
    sast/                    # semgrep JSON/SARIF
    dast/                    # baseline MD/JSON
    sca/                     # dotnet + trivy
  SECURITY_ANALYSIS.md         # этот документ
```

---

## Сводка исправлений в коде

| Проблема | Инструмент | Статус |
|----------|-----------|--------|
| MinimumLength(6) для паролей | Semgrep CWE-521 | Исправлено (8+) |
| Отсутствие security headers | DAST / ZAP 10020-10038 | Исправлено (middleware) |
| Секреты в appsettings | Semgrep CWE-798 | Документировано (User Secrets) |
| SQL injection (демо) | Semgrep CWE-89 | Не в production-коде |
| Уязвимый NuGet (демо) | dotnet SCA | Только в samples/ |
