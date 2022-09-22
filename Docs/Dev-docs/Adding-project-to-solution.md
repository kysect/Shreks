# Adding project to solution

Правила структуры солюшена в первую очередь соответствуют общим правилам Kysect - https://github.com/kysect/CodeRules/blob/master/Guides/SolutionStructure.md.

При добавлении нового проекта в солюшен нужно:

- Создать csproj в нужной директории
- В csproj добавить `<AnalysisLevel>latest</AnalysisLevel>`
- В csproj добавить `<EnforceCodeStyleInBuild>True</EnforceCodeStyleInBuild>`
- В Docker/build.dockerfile добавить копирование нового проекта аналогично
