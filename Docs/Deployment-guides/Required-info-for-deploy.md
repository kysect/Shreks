# Информация по развёртыванию

Как обновить тестовый стенд:

1. Получить доступ к Облаку
2. Получить SSH ключ от VM
3. Подключиться к VM
4. Перейти в директорию с репозиторием, выполнить git pull
5. Выполнить скрипт запуска тестового стенда - YaCloud/start.py

## Список sensitive info

- SSH от VM
- User и password к PostgeSQL
- Логин и пароль от админского аккаунта Shreks
- Client secret от Google service account

## Инструменты, к которым нужен доступ

- Облако
- VM в облаке
- Managed PostgreSQL server
- GitHub app
- Google drive, Id Google drive директории и ссылка на него
