# Tech Docs

## Rich entities

Если вы обнаружили класс, реализующий интерфейс `IEntity<T>`, то вам сюда: [Библиотека RichEntity](./UseRichEntity.md)

## GitHub OAuth

Для получения информации об аккаунте GitHub'а используется авторизация посредством OAuth.

Когда пользователь отправляет запрос на регистрацию в системе Shreks, он перенаправляется на страницу авторизации GitHub'а. После успешной авторизации username пользователя добавляется в базу данных Shreks в качестве `GithubAssociation` у `User`.

Для осуществления авторизации посредством OAuth имеется зарегистрированное `GitHub OAuth` приложение для `Shreks`, которое и будет запрашивать доступ к данным пользователя при авторизации.

Для того, чтобы WepApi приложение запустилось, необходимо добавить в него user secrets для связи с приложением GitHub OAuth. Пользовательские секреты содержат ClientId (идентификатор клиента, с которым взаимодействует приложение GitHub OAuth) и ClientSecret (некий токен, который необходимо куда-то сохранить или перегенерировать в случае потери).

С известными ClientId и ClientSecret следует открыть терминал и прописать следующие команды:

```
dotnet user-secrets init --project "полный путь к проекту WebApi"
dotnet user-secrets set "ShreksConfiguration:GithubConfiguration:OAuthClientId" "значение ClientId" --project "полный путь к проекту WebApi"
dotnet user-secrets set "ShreksConfiguration:GithubConfiguration:OAuthClientSecret" "значение ClientSecret" --project
``` "полный путь к проекту WebApi"