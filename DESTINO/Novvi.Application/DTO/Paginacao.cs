using Novvi.Application.Exceptions;

namespace Novvi.Application.DTO;

/// <summary>Regras e padrões de paginação da listagem v2 (CP5).</summary>
public static class Paginacao
{
    public const int PageSizePadrao = 20;
    public const int PageSizeMaximo = 100;

    /// <summary>page &gt;= 1 e 1 &lt;= pageSize &lt;= 100; caso contrário lança <see cref="PaginacaoInvalidaException"/>.</summary>
    public static void Validar(int page, int pageSize)
    {
        if (page < 1)
            throw new PaginacaoInvalidaException($"O parâmetro 'page' deve ser um inteiro maior ou igual a 1 (recebido: {page}).");

        if (pageSize < 1 || pageSize > PageSizeMaximo)
            throw new PaginacaoInvalidaException(
                $"O parâmetro 'pageSize' deve estar entre 1 e {PageSizeMaximo} (recebido: {pageSize}).");
    }
}
