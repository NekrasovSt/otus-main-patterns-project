namespace LinkServer;

/// <summary>
/// Правило с предикатом
/// </summary>
public class CompiledRule
{
    /// <summary>
    /// Ид
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Ссылка для перехода
    /// </summary>
    public required string Link { get; set; }
    
    /// <summary>
    /// Скомпилированный делегат
    /// </summary>
    public required Func<Dictionary<string, object>, bool> Predicate { get; set; }
}