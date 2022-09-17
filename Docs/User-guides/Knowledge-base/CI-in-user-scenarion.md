# CI

Github предоставляет функционал Github Action - встроенный CI для автоматического запуска и проверки добавленного кода. Он позволяет автоматически проверять базовые элементы в PR. Github action конфигурируются yml-файлами, которые должны лежать в директории `/.github/workflows/`. Их не нужно дополнительно включать на каждом репозитории, а значит достаточно будет добавить такой yml-файл в шаблонный репозиторий и он будет клонироваться во все остальные репозитории и запускать на них CI.

Что в CI можно конфигурировать в контексте автоматической проверки заданий:

- Сборка проекта.
- Запуск и проверка тестов.
- Запуск анализаторов и линтеров (если доступно для языка).

Пример yml-конфигурации, которая автоматически запускает сборку проекта и проверка на тестах:

```yml
name: .NET Core

on:
  push:
    branches: [ master ]
  pull_request:
    branches: [ master ]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET 6
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 6.0.x
    - name: Install dependencies
      run: dotnet restore Source/Kysect.Shreks.sln
    - name: Build
      run: dotnet build Source/Kysect.Shreks.sln --configuration Release --no-restore
    - name: Test
      run: dotnet test Source/Kysect.Shreks.sln --no-restore --verbosity normal
```
