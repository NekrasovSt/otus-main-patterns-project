namespace ServiceUtils.Exceptions;

public class EntityAlreadyExistException : Exception
{
    public EntityAlreadyExistException(string message) : base(message)
    {
    }

    public required string PropertyName { get; set; }
}