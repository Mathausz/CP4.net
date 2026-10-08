namespace Novvi.Application.DTO;

/// <summary>Envelope paginado devolvido pela listagem v2 (CP5).</summary>
public record PagedResponse<T>(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyList<T> Items)
{
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public static PagedResponse<T> Criar(int page, int pageSize, int totalItems, IReadOnlyList<T> items)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResponse<T>(page, pageSize, totalItems, totalPages, items);
    }
}
