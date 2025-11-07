namespace ServiceUtils.Exceptions;

public class InvalidPropertyException: Exception
{
    public InvalidPropertyException(string massage) : base(massage)
    {
    }
    public required string PropertyName { get; set; }
    public string? Id { get; set; }
}