namespace reviewer_service.Exceptions;

public class ProvidedFileIsNotYamlException : Exception
{
    public ProvidedFileIsNotYamlException(string? message) : base(message)
    {
    }
}