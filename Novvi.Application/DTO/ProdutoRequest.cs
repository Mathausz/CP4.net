namespace Novvi.Application.DTO;

/// <summary>Dados para criação/atualização de um produto.</summary>
public class ProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public double Preco { get; set; }
}
