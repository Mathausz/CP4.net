namespace Novvi.Application.DTO;

/// <summary>Dados para criação de um pedido.</summary>
public class PedidoRequest
{
    public Guid IdUsuario { get; set; }
    public Guid IdFuncionario { get; set; }
    public List<Guid> ProdutosIds { get; set; } = new();
}
