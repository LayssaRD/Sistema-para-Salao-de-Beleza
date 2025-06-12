namespace GestaoSalaoDeBeleza.Models;

public class ServicoOfertado
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public TimeSpan Duracao { get; set; }

    protected ServicoOfertado(string nome, decimal preco, TimeSpan duracao)
    {
        Nome = nome;
        Preco = preco;
        Duracao = duracao;
    }

    public override string ToString()
    {
        return $"{Nome} - R${Preco} - Duração: {Duracao.TotalMinutes} minutos";
    }
}
