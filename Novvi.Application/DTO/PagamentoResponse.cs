using Novvi.Domain.Entities;
using Novvi.Domain.Enums;

namespace Novvi.Application.DTO;

/// <summary>Dados de um pagamento retornados pela API.</summary>
public record PagamentoResponse(Guid Id, TipoPagamento TipoPagamento, DateTime Horario)
{
    public static PagamentoResponse FromDomain(Pagamento pagamento) =>
        new(pagamento.Id, pagamento.TipoPagamento, pagamento.Horario);
}
