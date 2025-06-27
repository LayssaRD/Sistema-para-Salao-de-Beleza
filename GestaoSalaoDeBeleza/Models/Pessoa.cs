using System.ComponentModel.DataAnnotations;
using GestaoSalaoDeBeleza.Interfaces;

namespace GestaoSalaoDeBeleza.Models;

public abstract class Pessoa : INotification
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    public Pessoa() {}

    protected Pessoa(string nome, string telefone, string email, DateTime dataNascimento)
    {
        Nome = nome;
        Telefone = telefone;
        Email = email;
        DataNascimento = dataNascimento;
    }

    public int CalcularIdade()
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - DataNascimento.Year;
        if (DataNascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade;
    }

    public virtual void Notificar(Pessoa pessoa)
    {
        Console.WriteLine($"Nome: {Nome} - Telefone: {Telefone} - E-mail: {Email} - Data de Nascimento: {DataNascimento:dd/MM/yyyy}");
    }
}