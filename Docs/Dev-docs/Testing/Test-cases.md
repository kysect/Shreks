# Test cases

## Фоновый github worker

- Студент добавлен в систему, но не добавлен в GitHub организацию - отправить инвайт
- Студент приглашён, но не принял инвайт - не отправлять повторно
- Студент принял инвайт, но не создал репозиторий - создать репозиторий из шаблона, указанного для организации
- Студент имеет репозиторий - не пытаться создавать новый репозиторий

## Действия с PR

- Студент создаёт pull request - система создаёт сабмишен для него
- Студент создаёт pull request, уже имея сабмишен по текущей работе - система уведомляет о том, что сабмишен уже создан и
  нужно вернуться к прошлому PR
- Студент создаёт pull request не в основую ветку (master) - система не создаёт сабмишен, а выводит предупреждение о том,
  что нужно создать PR в master
- Ментор создаёт PR - система определяет студента по названию репозитория и создаёт для него сабмишен
- Пользователь, который не является ментором и автором репозитория, создаёт PR - система сообщает о том, что у
  пользователя нет прав на выполнение команд в данном репозитории и не создаёт сабмишен
- Студент создаёт PR с неправильным названием ветки - система сообщает о том, что ветки должны называться в соответствии
  с шаблоном и предлагает пересоздать PR указывая список доступных названий веток

- Студент закрывает PR, в котором последний сабмишен оценён - ничего не происходит
- Студент закрывает PR, в котором последний сабмишен не оценён - система уведомляет о том, что сабмишен должен быть
  оценён, переводит его в inactive

- Студент переоткрывает PR, в котором последний сабмишен не активный - система делает его активным
- Студент переоткрывает PR, в котором последний сабмишен оценён или активный - ничего не происходит
- Ментор переоткрывает PR - ничего не происходит

## Действия с комитами

- Студент пушит коммит в PR, в котором нет активного сабмишена - создаётся сабмишен
- Студент пушит коммит в PR, в котором уже есть активный сабмишен - обновляется дата создания сабмишена
- Ментор пушит коммит в PR - ситсема сообщает, что не триггерится на коммиты ментора

## Очередь

- Студент создаёт сабмишен - очередь его группы пересоздаётся, чтобы добавить его в очередь
- Студент переводит сабмишен в active - очередь его группы пересоздаётся, чтобы добавить его в очередь
- Студент переводит сабмишен в inactive - очередь его группы пересоздаётся, чтобы убрать его из очереди
- Ментор оценивает сабмишен - очередь его группы пересоздаётся, чтобы убрать его из очереди
- Студент пушит новый коммит - у его сабмишена обновляется дата, очередь перестраивается, чтобы обновить его позицию в
  очереди
- Ментор обновляет дату сабмишена, который ещё не оценён - очередь перестраивается, чтобы обновить позицию студента в
  очереди

## Таблица с баллами

- Ментор оценивает работу - система перегенерирует таблицу с баллами
- Ментор обновляет баллы или дату сабмишена - система перегенерирует таблицу с баллами

## Команды

- Студент отправляет команду, которая доступна только ментору - система сообщает об ошибке доступа (отсутствии прав)
- Ментор отправляет команду /rate в комментарий к PR или в ревью - система ставит оценку сабмишену
- Ментор отправляет команду /rate сабмишену, который уже оценён - система сообщает об ошибке и о том, что для этого есть
  команда Update
- Ментор отправляет команду /update в комментарий к PR или в ревью - система обновляет оценку сабмишену
- Ментор отправляет команду /update сабмишену, который ещё не был оценён - система сообщает об ошибке и о том, что для
  этого есть команда rate
- Ментор в комменатрии к ревью в первой строке пишет оценку, а дальше пишет комментарий - система парсит команду только
  из первой строки
- Ментор отправляет request changes без оценки - система сообщает, что нужно оценить работу
