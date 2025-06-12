using System;
using GenericRepository.Entidade;

namespace GestaoSalaoDeBeleza.Models;

public class Cliente : Pessoa, IEntidade
{
    public string Endereco { get; set; }
    public List<Agendamento> HistoricoAgendamentos { get; set; } = new();

    public Cliente() : base("", "", "", DateTime.MinValue)
    {
        Endereco = "";
    }
    public Cliente(string nome, string telefone, string email, DateTime dataNascimento, string endereco)
        : base(nome, telefone, email, dataNascimento)
    {
        Endereco = endereco;
    }

    public int CalcularIdade()
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - DataNascimento.Year;
        if (DataNascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade;
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
