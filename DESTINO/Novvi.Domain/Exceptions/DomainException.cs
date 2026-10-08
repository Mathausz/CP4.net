namespace Novvi.Domain.Exceptions;

/// <summary>Representa a violação de uma regra de negócio do domínio.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
