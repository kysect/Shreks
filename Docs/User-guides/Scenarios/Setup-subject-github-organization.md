# Процесс инициализации GitHub организации для дисциплины

Цель: Предоставить студентам доступ к организации, куда они будут загружать работы.

Процесс:

1. Преподаватель создаёт шаблонный репозиторий для дисциплины
2. Преподаватель заполняет форму обращения для добавления дисциплины в систему
3. Преподаватель обращается в поддержку сервиса и запрашивает создание организации
4. Поддержка сервиса добавляет данные о дисциплине и включает возможность к ней присоединиться
5. Поддержка добавляет преподавателей и менторов в организацию и предоставляет права на чтение приватных репозиториев
6. Студенты авторизуются в системе, используя свои учётные записи, аффилированные с университетом, и учётные записи GitHub, которые они будут использовать в ходе курса. (*)
7. Система начинает обработку пользователей:
   1. Отправляет приглашения в организацию (**)
   2. Создаёт для каждого студента личный приватный репозиторий из шаблона
8. Студенты принимают приглашение и получают доступ к репозиторию в организации

Дополнительная информация:

- Приглашения в организацию отправляются GitHub'ом. Для этого он использует почту, указанную в настройках пользователя.
- Если в течение семи дней студент не принимает приглашение в организацию, то оно устаревает и больше не является актуальным. В таком случае нужно обратиться в поддержку и запросить повторную отправку приглашения.
- В процессе прохождения курса очень не рекомендуется изменять github username, но если так произошло, то стоит обратиться в поддержку и предоставить ей данные об изменениях.
- (*) Интеграция с ITMO ID находиться в процессе разработки. На данный момент поддерживается только ручное добавление студентов в систему.
- (**) У GitHub есть ограничение в 50 приглашений в день. Система каждый день высылает приглашения пока не сталкивается с лимитом.

## Формат предоставления информации о дисциплине

- Название дисциплины и список групп, которые будут подключены к системе автоматизации
- Список из ФИО и github usernames всех преподавателей, которые будут вести данную дисциплину
- Список заданий для выполнения и данные по ним - номер, название, разбаловка, дедлайн
- Количество прочих работ - контрольные, рубежки, экзамен, и баллы за них
- Политика работы с дедлайнами
- Политика приоритизации очереди принятия студентов
- Ссылка на шаблонный репозиторий
- Документ с описанием необходимого ПО и гайд по настройке окружения