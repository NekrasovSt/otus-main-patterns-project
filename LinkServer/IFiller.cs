namespace LinkServer;

/// <summary>
/// Заполнитель запроса
/// </summary>
public interface IFiller
{
    /// <summary>
    /// Сформировать словарь дополнительных данных
    /// </summary>
    IReadOnlyDictionary<string, object> Fill();
}