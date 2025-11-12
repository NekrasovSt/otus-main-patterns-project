using System.Diagnostics.CodeAnalysis;

namespace ServiceUtils.Exceptions;

[ExcludeFromCodeCoverage]
public class EntityAlreadyExistException(string message) : Exception(message)
{
    public required string PropertyName { get; set; }
}