namespace GestaoSalaoDeBeleza.Models;

public class ServicoOfertado
{
    public int ServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoMinutos { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public List<Agendamento> Agendamentos { get; set; } = new();
}
