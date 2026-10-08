namespace Novvi.Application.DTO;

/// <summary>Dados para criação/atualização de um usuário.</summary>
public class UsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<EnderecoRequest> Enderecos { get; set; } = new();
}
