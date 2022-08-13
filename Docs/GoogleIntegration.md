# Google integration

GoogleTableAccessor содержит в себе методы UpdatePointsAsync и UpdateQueueAsync, создающие таблицы на листах "Баллы" и "Очередь" соответсвенно (Создает листы если их не было)

Пример использования находится в проекте Playground.Google:

При конфигурации сервисов кроме экстеншиона `.AddGoogleIntegration` необходимо также передать:

- ICultureInfoProvider
- IUserFullNameFormatter
- ISpreadsheetIdProvider
- SheetsService

Создание таблицы:
- Создать таблицу в https://docs.google.com/spreadsheets
- Получить spreadsheet ID из ссылки docs.google.com/spreadsheets/d/**1buz1imYXq7ijn8DFThyB-j59BP-L8Sj_t3EmJBOar90**/edit#gid=0
- Создать ConstSpreadsheetIdProvider

Создание api:

- Создать проект в https://console.cloud.google.com/
- Включить Google Sheets API https://console.cloud.google.com/apis/library/sheets.googleapis.com
- Создать Service Account Credentials, дать доступ на изменение в созданной ранее гугл таблице `email`у аккаунта
- Скачать ключи в форме JSON, переименовать client_secrets.json и положить в Playground.Google

Примеры получившихся таблиц:

Очередь:

![image](https://user-images.githubusercontent.com/70411602/184488611-f9b7e945-5991-4057-a8b3-f4cc0cf269a2.png)

Баллы:

![image](https://user-images.githubusercontent.com/70411602/184488720-ea5c32a0-4ff6-491d-a2c5-91f74ec61e82.png)
