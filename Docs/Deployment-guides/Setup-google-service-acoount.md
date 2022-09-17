# Setup Google service account

1. Создать проект в https://console.cloud.google.com/
2. Включить Google Sheets API https://console.cloud.google.com/apis/library/sheets.googleapis.com
3. Включить Google Drive API https://console.cloud.google.com/apis/library/drive.googleapis.com
4. Создать Service Account Credentials
5. Создать директорию в доступном Drive и предоставить Editor права Service account'у указав его email
6. Скачать ключи для Service Account credential в форме JSON, содержимое положить в dotnet secrets как `GoogleIntegrationConfiguration:ClientSecrets`
7. Добавить в appsettings.json значение для `GoogleIntegrationConfiguration:GoogleDriveId` - id директории, которая будет использоваться для генерации в неё таблиц
