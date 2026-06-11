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



