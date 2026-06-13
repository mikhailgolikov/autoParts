# веб-приложение по продаже автозапчастей и расходников

# эндпоинты:

# Аутентификация
Для всех защищённых эндпоинтов необходимо передавать JWT токен в заголовке `Authorization`:

### AuthController

| Метод | Эндпоинт | Доступ | Описание |
|-------|----------|--------|----------|
| POST | `/api/auth/register` | Все | Регистрация нового клиента |
| POST | `/api/auth/login` | Все | Вход в систему, получение JWT токена |

#### Пример запроса: Регистрация
```json
POST /api/auth/register
{
  "name": "Иван Петров",
  "email": "ivan@example.com",
  "password": "secure123",
  "phoneNumber": "+79123456789"
}
```
#### Пример запроса: Логин
```json
POST /api/auth/login
{
  "email": "ivan@example.com",
  "password": "secure123"
}
```

# Пользователи
## UsersController
### /api/users/me GET, может только авторизированный пользователь(получить свой профиль)
### /api/users/me/profile PUT, может только авторизированный пользователь(обновить профиль - имя, телефон)
### /api/users/me/password PUT, может только авторизированный пользователь(сменить пароль)

#### Пример запроса обновить профиль
```json
PUT /api/users/me/profile
Authorization: Bearer {token}
{
  "name": "Иван Сидоров",
  "email": "ivan.new@example.com",
  "phoneNumber": "+79998887766"
}
```
#### Пример запроса сменить пароль
```json
PUT /api/users/me/password
Authorization: Bearer {token}
{
  "currentPassword": "secure123",
  "newPassword": "newSecure456"
}
```

# Администрирование
### /api/users GET, доступ имеет только админ(получаем спсиок всех пользователей)
### /api/users/{id} GET, имеют доступ админ или сам пользователь(получаем пользователя по id)
### /api/users POST, имеет доступ админ(создаем пользователя, любая роль)
### /api/users/{id} DELETE, имеет доступ только админ(Удалить пользователя)

#### Пример запроса создать пользователя
```json
POST /api/users
Authorization: Bearer {admin-token}
{
  "name": "Петр Иванов",
  "email": "petr@example.com",
  "password": "password123",
  "phoneNumber": "+79001234567",
  "role": "Creator"
}
```
#### Пример запроса сменить роль
```json
PATCH /api/users/{userId}/role
Authorization: Bearer {admin-token}
{
  "role": "Creator"
}
```


# Компания
## CompanyController
### /api/company GET, общий доступ, Получить информацию о компании + контакты
### /api/company PUT, доступ - Admin, Creator, Обновить информацию о компании


## ContactsController
### /api/company/contacts GET, общий доступ, получить все контакты
### /api/company/contacts/{id} GET, общий доступ, получить контакт по id
### /api/company/contacts POST, ДОСТУП - Admin, Creator, Создать контакт
### /api/company/contacts/{id} PUT, доступ - Admin, Creator, обновить контакт
### /api/company/contacts/{id} DELETE, доступ - Admin, Creator, удалить контакт 

#### Пример запроса обновить компанию
```json
PUT /api/company
Authorization: Bearer {admin-token}
{
  "name": "Авто",
  "description": "Крупнейший магазин автозапчастей",
  "address": "г. Москва, ул. Автомобилистов, д. 15"
}
```
#### Пример запроса создать контакт 
```json
POST /api/company/contacts
Authorization: Bearer {admin-token}
{
  "name": "+7 (495) 123-45-67",
  "description": "Отдел продаж"
}
```
#### Пример запроса получить компанию
``` json
GET /api/company
{
  "name": "Автоиномир",
  "description": "Крупнейший магазин автозапчастей",
  "address": "г. Москва, ул. Автомобилистов, д. 15",
  "contacts": [
    {
      "id": "22222222-2222-2222-2222-222222222222",
      "name": "+7 (495) 123-45-67",
      "description": "Отдел продаж"
    },
    {
      "id": "33333333-3333-3333-3333-333333333333",
      "name": "https://t.me/avtonomir",
      "description": "Telegram канал"
    }
  ]
}
```

# Новости
## NewsController

| Метод | Эндпоинт | Доступ | Описание |
|-------|----------|--------|----------|
| GET | `/api/news` | Все | Получить все новости |
| GET | `/api/news/{id}` | Все | Получить новость по ID |
| POST | `/api/news` | `Admin`, `Creator` | Создать новость |
| PUT | `/api/news/{id}` | `Admin`, `Creator` | Обновить новость |
| DELETE | `/api/news/{id}` | `Admin`, `Creator` | Удалить новость |

### Пример запроса: Создать новость
```json
POST /api/news
Authorization: Bearer {token}
{
  "name": "Новое поступление запчастей",
  "description": "Поступили оригинальные детали Toyota и BMW",
  "publishedAt": "2026-06-11T12:00:00Z",
  "imagePath": "/uploads/news/stock.jpg"
}
```

# Акции 
## PromotionController
### /api/promotions GET, доступ имеют все, Получить все акции
### /api/promotions/{id} GET, доступ имеют все, Получить акцию по ID
### /api/promotions POST, доступ имеют Admin и Creator, Создать акцию
### /api/promotions/{id} PUT, доступ имеют Admin и Creator, Обновить акцию
### /api/promotions/{id} DELETE, доступ имеют Admin и Creator, Удалить акцию

### Пример запроса: Создать акцию
```json
POST /api/promotions
Authorization: Bearer {token}
{
  "name": "Скидка 15% на тормозные колодки",
  "description": "До конца месяца",
  "imagePath": "/uploads/promotions/brakes-sale.jpg"
}
```

# Сертификаты

## CertificateController

| Метод | Эндпоинт | Доступ | Описание |
|-------|----------|--------|----------|
| GET | `/api/certificates` | Все | Получить все сертификаты |
| GET | `/api/certificates/{id}` | Все | Получить сертификат по ID |
| POST | `/api/certificates` | `Admin`, `Creator` | Создать сертификат |
| PUT | `/api/certificates/{id}` | `Admin`, `Creator` | Обновить сертификат |
| DELETE | `/api/certificates/{id}` | `Admin`, `Creator` | Удалить сертификат |

### Пример запроса: Создать сертификат
```json
POST /api/certificates
Authorization: Bearer {token}
{
  "name": "Сертификат ISO 9001",
  "imagePath": "/uploads/certificates/iso9001.jpg"
}
```

# Обращения

## AppealsController

### Создание обращений

| Метод | Эндпоинт | Доступ | Описание |
|-------|----------|--------|----------|
| POST | `/api/appeals/client-questions` | Авторизованный | Создать вопрос клиента |
| POST | `/api/appeals/supplier-requests` | Авторизованный | Создать заявку поставщика |

#### Пример: Вопрос клиента
```json
POST /api/appeals/client-questions
Authorization: Bearer {token}
{
  "category": "Order",
  "managerComment": "Нужны тормозные колодки",
  "contactPhone": "+79001234567",
  "contactEmail": "ivan@mail.ru"
}
```
#### Пример: Заявка поставщика
```json POST /api/appeals/supplier-requests
Authorization: Bearer {token}
{
  "companyName": "ООО Поставщик",
  "managerComment": "Хотим стать партнёрами"
}
```
### /api/appeals/my GET, доступ авт пользователь Авторизованный, просмотр клиентом своих обращений
### /api/appeals GET, доступ admin, creator, просмотр всех обращений пользователей 
### /api/appeals/{id} GET, доступ - admin, creator, владелец, образение к конкретному образению 
### /api/appeals/user/{userId} GET, доустп admin, creator, обращения пользователя

### /api/appeals/{id}/status PATCH, Admin, Creator, сменить статус
### /api/appeals/{id}/contacts PATCH, Admin, Creator, владелец



# Бренды
## BrandsController

### /api/brands  GET, получить список всех брендов, могут все
### /api/brands/{id} GET, Получить бренд по ID, доступ имеют все
### /api/brands POST, Создать бренд. Доступ: Admin, Creator.
### /api/brands/{id} PUT, Обновить бренд. Доступ: Admin, Creator.
### /api/brands/{id} DELETE, Удалить бренд, Доступ: Admin, Creator.

# Категории
## CategoriesController
### /api/categories GET, Получить список всех категорий, общий доступ
### /api/categories/{id} GET, Получить категорию по ID вместе со всеми атрибутами этой категории, общий доступ
### /api/categories POST, Создать категорию. Доступ: Admin, Creator.
### /api/categories/{id} PUT, Обновить категорию. Доступ: Admin, Creator.
### /api/categories/{id} DELETE, Удалить категорию,  Доступ: Admin, Creator.  Важно: Нельзя удалить категорию, если к ней привязаны товары.

# Атрибуты 
## AttributesController
### /api/attributes GET, Получить список всех атрибутов, общий доступ
### /api/attributes/{id}, GET, получить атрибут по id, общий доступ
### /api/attributes/category/{categoryId} GET, получить все атрибуты конкретной категории, общий доступ
### /api/attributes POST, Создать атрибут. Доступ: Admin, Creator.
### /api/attributes/{id}, PUT, Обновить атрибут. Доступ: Admin, Creator.
### /api/attributes/{id}, DELETE, Удалить атрибут. Доступ: Только Admin, Creator. Важно: При удалении атрибута все его значения у товаров удаляются автоматически.

# Товары
## ProductsController
### /api/products GET, Получить список товаров с фильтрацией, пагинацией и сортировкой, общий доступ
### /api/products/{id}, GET Получить полную информацию о товаре, включая все значения атрибутов, абщий доступ
### /api/products/search?q={text} GET, полный доступ, Поиск товаров по названию и артикулу.
### /api/products/brand/{brandId} GET, получить все бренды товара, полный доступ
### /api/products/category/{categoryId} GET, получить все товары по категории, обзий доступ
### /api/products POST, Создать товар. Доступ: Admin, Creator.
### PUT /api/products/{id}, Полностью обновить товар. Доступ: Admin, Creator.
### PATCH /api/products/{id}/stock, Обновить количество на складе. Доступ: Admin, Creator.
### PATCH /api/products/{id}/price, Обновить цену. Доступ: Admin, Creator.
### DELETE /api/products/{id}, Удалить товар. Доступ: Только Admin и Creator
