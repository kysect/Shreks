# Shreks

Система предназначена для автоматизации процесса обучения, а именно: процессов отправки заданий студентами, формирования очереди и оценивания заданий преподавателями.

- [Glossary](Docs/Glossary.md)
- [Tech docs](Docs/TechDocs/Readme.md)

## Idea

Образовательный процесс на дисциплинах связанных с программированием требует написания и ревью код. Для этого хорошо подходит Github с его Pull request и Review request системами. Но в отличии от обычной разработки, в контексте образования важно отслеживать прогресс и оценивать работу студентов. Shreks - это сервис, который за счёт Github app и прослушивания Webhooks с Github. В качестве Presentation layer система использует Google Sheet. В директории Google Drive генерируются таблицы успеваемости и таблицы с очередями на сдачу.

## How to config infrastructure

1. Настроить Google integration - [Google integration](Docs/TechDocs/GoogleIntegration.md#how-to-build)
2. Настроить Github OAuth - [Github OAuth](Docs/TechDocs/GithubSetup.md)
3. Настроить Github App - [Github App](Docs/TechDocs/GithubSetup.md#how-to-config-github-app)