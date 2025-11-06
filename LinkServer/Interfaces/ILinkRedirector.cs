namespace LinkServer.Services;

public interface ILinkRedirector
{
    Task<string> RedirectAsync(Dictionary<string, object>? dictionary, CancellationToken token);
}