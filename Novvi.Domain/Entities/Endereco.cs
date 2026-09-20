using Novvi.Domain.Exceptions;

namespace Novvi.Domain.Entities;

/// <summary>Endereço de entrega de um usuário. N:1 -&gt; Usuario.</summary>
public class Endereco
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Cep { get; private set; }
    public string Complemento { get; private set; }

    public Guid IdUsuario { get; private set; }

    protected Endereco() { }

    public Endereco(string cep, string complemento, Guid idUsuario)
    {
        if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
            throw new DomainException("Cep inválido");
        Cep = cep;

        if (string.IsNullOrWhiteSpace(complemento) || complemento.Length < 2)
            throw new DomainException("Complemento incompleto");
        Complemento = complemento;

        if (idUsuario == Guid.Empty)
            throw new DomainException("Endereço precisa estar associado a um usuário");
        IdUsuario = idUsuario;
    }

    public override string ToString() => $"{Cep} - {Complemento}";
}
