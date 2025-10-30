namespace ServiceUtils.Exceptions;

public class EntityNotException : Exception
{
    public EntityNotException(string id) : base($"Сущность с ид {id} не найдена")
    {
        Id = id;
    }

    public string Id { get; set; }
}