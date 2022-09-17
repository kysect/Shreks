# Github setup

## How to config Github OAuth

Для получения информации об аккаунте GitHub'а используется авторизация посредством OAuth.

Когда пользователь отправляет запрос на регистрацию в системе Shreks, он перенаправляется на страницу авторизации GitHub'а. После успешной авторизации username пользователя добавляется в базу данных Shreks в качестве `GithubAssociation` у `User`.

Для осуществления авторизации посредством OAuth имеется зарегистрированное `GitHub OAuth` приложение для `Shreks`, которое и будет запрашивать доступ к данным пользователя при авторизации.

Для того, чтобы WepApi приложение запустилось, необходимо добавить в него user secrets для связи с приложением GitHub OAuth. Пользовательские секреты содержат ClientId (идентификатор клиента, с которым взаимодействует приложение GitHub OAuth) и ClientSecret (некий токен, который необходимо куда-то сохранить или перегенерировать в случае потери).

С известными ClientId и ClientSecret следует открыть терминал и прописать следующие команды:

```
dotnet user-secrets init --project "полный путь к проекту WebApi"
dotnet user-secrets set "GithubIntegrationConfiguration:GithubAuthConfiguration:OAuthClientId" "значение ClientId" --project "полный путь к проекту WebApi"
dotnet user-secrets set "GithubIntegrationConfiguration:GithubAuthConfiguration:OAuthClientSecret" "значение ClientSecret" --project "полный путь к проекту WebApi"
```

В качестве альтернативы можно добавить эти два значения в `appsettings.json`.

## How to config Github App

1. Go to Organization settings > Developer settings > Github Apps > New Github App
2. Setup name, Webhook URL where you will listen webhooks
3. Add read and write repository permission for:
   1. Administration
   2. Commit statuses
   3. Contents
   4. Discussions
   5. Issues
   6. Packages
   7. Pull requests
4. Add read and write organization permissions:
   1. Members
5. Subscribe to permissions:
   1. Commit comment
   2. Issue comment
   3. Discussion comment
   4. Pull request
   5. Pull request review
   6. Pull request review comments
   7. Push
6. Add Webhook secret and save to dotnet secrets as `GithubIntegrationConfiguration:GithubAppConfiguration:GithubAppSecret`
7. Generate private key (*.pem file) and save to dotnet secrets as `GithubIntegrationConfiguration:GithubAppConfiguration:PrivateKey`
8. After app creation add App ID into `appsettings.json` as `GithubIntegrationConfiguration:GithubAppConfiguration:AppIntegrationId`
9. Add `GithubIntegrationConfiguration:GithubAppConfiguration:ServiceOrganizationName` into `appsettings.json` and set this field with the value `kysect`.
10. If you need to test Github App in another organization, you need to make your app public in Github App settings
11. Add App into your test organization (go to https://github.com/apps/your-app-name)
