namespace Novvi.Application.Exceptions;

/// <summary>Representa uma falha ao acessar a camada de persistência (banco indisponível, etc.).</summary>
public class PersistenceException : Exception
{
    public PersistenceException(string message, Exception inner) : base(message, inner)
    {
    }
}
