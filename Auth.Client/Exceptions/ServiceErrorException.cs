using Auth.Client.Interfaces;

namespace Auth.Client;

public class ServiceErrorException : Exception
{
    public string? Id { get; set; }

    public ServiceErrorException(string? id, string message) : base(message)
    {
        Id = id;
    }
}