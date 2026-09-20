namespace Novvi.Application.DTO;

/// <summary>Dados para cadastro de um endereço.</summary>
public class EnderecoRequest
{
    public string Cep { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
}
