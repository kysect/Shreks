# Google integration

ITableUpdateQueue, представляемый бэкграунд воркером GoogleTableUpdateWorker содержит в себе методы EnqueueCoursePointsUpdate и EnqueueSubmissionsQueueUpdate, добавляющие курс для обновления его таблицы на листах "Баллы" и "Очередь" соответственно (Создает листы если их не было)
Обновлении таблиц происходит с периодичностью в одну минуту
Также если у курса нет ассоциации с таблицей, создает таблицу в драйве и записывает новую ассоциацию в базу.

Пример использования находится в проекте Playground.Google:

При конфигурации сервисов кроме экстеншиона `.AddGoogleIntegration` необходимо также передать:

- ICultureInfoProvider
- IUserFullNameFormatter
- Базу данных
- Хэндлеры
- Маппинг
- Логгер

И конфигурировать через `options` Google Credentials и DriveId (Id нужной папки)

Создание api:

- Создать проект в https://console.cloud.google.com/
- Включить Google Sheets API https://console.cloud.google.com/apis/library/sheets.googleapis.com
- Включить Google Drive API https://console.cloud.google.com/apis/library/drive.googleapis.com
- Создать Service Account Credentials, дать в драйве роль Editor `email`у аккаунта
- Скачать ключи в форме JSON, переименовать client_secrets.json и положить в Playground.Google

Примеры получившихся таблиц:

Очередь:

![image](https://user-images.githubusercontent.com/70411602/184488611-f9b7e945-5991-4057-a8b3-f4cc0cf269a2.png)

Баллы:

![image](https://user-images.githubusercontent.com/70411602/184488720-ea5c32a0-4ff6-491d-a2c5-91f74ec61e82.png)
