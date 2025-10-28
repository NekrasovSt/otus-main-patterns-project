namespace RuleEditor.Exceptions;

public class EntityAlreadyExistException : Exception
{
    public EntityAlreadyExistException(string message) : base(message)
    {
    }

    public string PropertyName { get; set; }
}