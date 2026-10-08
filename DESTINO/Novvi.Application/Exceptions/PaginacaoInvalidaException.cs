namespace Novvi.Application.Exceptions;

/// <summary>Parâmetros de paginação (page / pageSize) fora da regra. Mapeada para HTTP 400 (CP5).</summary>
public class PaginacaoInvalidaException(string message) : Exception(message);
