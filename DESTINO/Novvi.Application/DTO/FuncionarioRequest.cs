namespace Novvi.Application.DTO;

/// <summary>Dados para criação/atualização de um funcionário.</summary>
public class FuncionarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public double Salario { get; set; }
}
