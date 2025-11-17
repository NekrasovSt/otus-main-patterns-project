using System.Diagnostics.CodeAnalysis;

namespace ServiceUtils.Exceptions;

[ExcludeFromCodeCoverage]
public class EntityNotFoundException(string id) : Exception($"Сущность с ид {id} не найдена")
{
    public string Id { get; set; } = id;
}