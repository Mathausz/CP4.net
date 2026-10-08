using Novvi.Domain.Enums;

namespace Novvi.Application.DTO;

/// <summary>Dados para registrar o pagamento de um pedido.</summary>
public class PagamentoRequest
{
    public TipoPagamento TipoPagamento { get; set; }
}
