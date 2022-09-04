# Google integration

## Details

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

И конфигурировать через `options` Google Credentials и DriveId (Id нужной папки).

Примеры получившихся таблиц:

Очередь:

![image](https://user-images.githubusercontent.com/70411602/184488611-f9b7e945-5991-4057-a8b3-f4cc0cf269a2.png)

Баллы:

![image](https://user-images.githubusercontent.com/70411602/184488720-ea5c32a0-4ff6-491d-a2c5-91f74ec61e82.png)

## How to build

> Если вы занимаетесь разработкой, то можете обратиться и получить доступ к уже сгенерированным проектам

1. Создать проект в https://console.cloud.google.com/
2. Включить Google Sheets API https://console.cloud.google.com/apis/library/sheets.googleapis.com
3. Включить Google Drive API https://console.cloud.google.com/apis/library/drive.googleapis.com
4. Создать Service Account Credentials
5. Создать в Google Drive директорию, дать сервис аккаунту к ней write доступ(роль Editor `email`у аккаунта выдать)
6. Скачать ключи для Service Account credential в форме JSON, переименовать client_secrets.json и положить в Kysect.Shreks.WebApi
7. Добавить в appsettings.json значение для `GoogleIntegrationConfiguration:GoogleDriveId` - id директории, которая будет использоваться для генерации в неё таблиц
