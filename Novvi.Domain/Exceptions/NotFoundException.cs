namespace Novvi.Domain.Exceptions;

/// <summary>Indica que o recurso solicitado não existe ou está inativo.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
