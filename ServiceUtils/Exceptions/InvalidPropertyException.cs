using System.Diagnostics.CodeAnalysis;

namespace ServiceUtils.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidPropertyException(string massage) : Exception(massage)
{
    public required string PropertyName { get; set; }
    public string? Id { get; set; }
}