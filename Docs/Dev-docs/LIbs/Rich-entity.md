# Rich entities

Доменные сущности реализованы с использованием сурс-генераторов для генерации бойлерплейт кода сущностей.
Идентификаторы, сравнение по ним.

Для `partial` типов, реализующих интерфейс `IEntity<TIdentifier>`, генерируется
свойтсво `TIdentifier Id { get; protected init; }` и методы `Equals`, `GetHashCode` по нему.

```csharp
public partial class User : IEntity<Guid>
```

Для `partial` типов, реализующих интерфейс `IEntity` по всем свойствам помеченым атрибутом
`KeyProperty`, чей тип реализует `IEntity<TIdentifier>` генерируются свойства типа `TIdentifier`
с именем, соответствующим имени свойства, к которому добавлено `Id` (ex: `User User` -> `Guid UserId`).

По всем этим сгенерированным свойсвам генерируются `Equals` и `GetHashCode` методы.
> Атрибут `[KeyProperty]` может быть использован и на типах, не реализующих интерфейс `IEntity<TIdentifier>`.
> Тогда дополнительных свойств сгенерированно не будет, но сгенерированные методы `Equals`
> и `GetHashCode` будут использовать их в своей реализации.
>
> В данном проекте эта операция не используется.

```csharp
public partial class GroupAssignment : IEntity
{
    ...
    
    [KeyProperty]
    public virtual StudentGroup Group { get; protected init; }

    [KeyProperty]
    public virtual Assignment Assignment { get; protected init; }
    
    ...
}
```

Более подробное описание с примерами генерируемого кода можно найти в README репозитория библиотеки
https://github.com/ronimizy/RichEntity