using Novvi.Domain.Enums;
using Novvi.Domain.Exceptions;

namespace Novvi.Domain.Entities;

/// <summary>Pagamento de um pedido. 1:1 -&gt; Pedido.</summary>
public class Pagamento
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public TipoPagamento TipoPagamento { get; private set; }
    public DateTime Horario { get; private set; } = DateTime.Now;

    public Guid IdPedido { get; private set; }

    protected Pagamento() { }

    public Pagamento(TipoPagamento tipoPagamento, Guid idPedido)
    {
        if (!Enum.IsDefined(typeof(TipoPagamento), tipoPagamento))
            throw new DomainException("Tipo de pagamento inválido");
        TipoPagamento = tipoPagamento;

        if (idPedido == Guid.Empty)
            throw new DomainException("Pagamento precisa estar associado a um pedido");
        IdPedido = idPedido;
    }

    public override string ToString() => $"Horario: {Horario:dd/MM/yyyy} - {TipoPagamento}";
}
