namespace LinkServer;

public interface IFiller
{
    IReadOnlyDictionary<string, object> Fill();
}