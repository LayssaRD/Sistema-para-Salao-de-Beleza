namespace GestaoSalaoDeBeleza.Models;

public class Cliente : Pessoa
{
    public string Endereco { get; set; } = string.Empty;
    public List<Agendamento> HistoricoAgendamentos { get; set; } = new();

    public Cliente() : base() { }

    public Cliente(string nome, string telefone, string email, DateTime dataNascimento, string endereco)
            : base(nome, telefone, email, dataNascimento)
    {
        Endereco = endereco;
    }

    public void AdicionarAgendamento(Agendamento agendamento)
    {
        HistoricoAgendamentos.Add(agendamento);
    }

    public override string ToString()
    {
        return $"ID: {Id}\nNome: {Nome}\nTelefone: {Telefone}\nEmail: {Email}" +
            $"\nData de Nascimento: {DataNascimento:dd/MM/yyyy}\nEndereço: {Endereco}";
    }
}
