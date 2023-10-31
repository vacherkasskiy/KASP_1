# Тестовое задание KASPERSKY №1 (усложненная версия)

## Условие

Требуется реализовать два приложения, используя .NET Core / .NET:

Сервисное приложение, реализующее простой REST API, который предоставляет ресурс для создания задач на получение ревьюеров для переданного пути и получения статуса задачи по идентификатору
утилиту, работающую из командной строки, отправляющую сервисному приложению команды на создание и просмотр состояния задач.
 
После завершения команды на получение ревьюеров должен быть выведен уникальный идентификатор задачи.
После завершения команды просмотра статуса задачи может быть выведено два результата:
- статус "задача еще выполняется"
- отчет о ревьюерах
 

Пример исполнения утилиты из командной строки:

```
reviewer_util add reviewers.yaml folder1/readme.md
```

> Task created with ID: 111

```
reviewer_util status 111
```

> Task 111 in progress

```
reviewer_util status 111
```

> path: folder1/readme.md  
> reviewers: user1; user2; user3

**Примечание:**
- Cервисное приложение не имеет постоянного хранилища состояния (каждый запуск как чистый лист)
- Cервисное приложение и утилита работают на одном и том же устройстве
- Pекомендуется максимальное использования (утилизация) вычислительных ресурсов устройства, на котором выполняется утилита.

## Руководство по использованию

- Клонировать репозиторий себе на ПК
- Открыть решение в любой IDE, поддерживающей язык C#
- Запустить проект `reviewer_service`
- В файле `Program.cs` проекта `reviewer_util` присвоить переменной `baseUrl` адрес, на котором располагается `reviewer_service`
- "Собрать" (build) проект `reviewer_util`
- Добавить путь до директории с файлом `reviewer_util.exe` в `PATH`

Команды могут быть двух типов:
- reviewer_util add <абсолютный путь к yaml файлу> <путь к файлу для нахождения ревьюверов относительно yaml>
- reviewer_util status <id задачи>

Пример:
```
reviewer_util add C:\Documents\Project\reviewers.yaml folder1/readme.md
reviewer_util status 1
```

Пример yaml файла:
```yaml
rules:
  default_rule:
    included_paths:
      - folder1/* # folder1/text.txt, folder1/README.md, etc
    reviewers:
      - user1
      - user2
        
  second_rule:
    included_paths:
      - folder2/*/*/*.txt # folder2/docs/texts/smth.txt, folder2/homework/kaspersky/description.txt, etc
    reviewers:
      - user4
      - user5

  additional_rule:
    included_paths:
      - folder1/*.md # folder1/README.md, folder1/notes.md, etc
    reviewers:
      - user3
```

## Примечания:

- API должно быть запущено вручную, утилита же запускается через коммандную строку
