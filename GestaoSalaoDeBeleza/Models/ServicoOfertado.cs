namespace GestaoSalaoDeBeleza.Models;

public class ServicoOfertado
{
    public int ServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoMinutos { get; set; }
    public CategoriaServico Categoria { get; set; }
    public List<Agendamento> Agendamentos { get; set; } = new();

    public override string ToString()
    {
        return $"ID: {ServicoId} - Nome: {Nome}\nPreço: R$ {Preco:F2} - Duração: {DuracaoMinutos} minutos - Categoria: {Categoria}";
    }
}

public enum CategoriaServico
{
    Cabelo,
    Unhas,
    Sobrancelha,
    Estetica
}
