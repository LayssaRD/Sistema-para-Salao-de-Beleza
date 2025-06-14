using System.ComponentModel.DataAnnotations;
using GestaoSalaoDeBeleza.Interfaces;

namespace GestaoSalaoDeBeleza.Models;

public abstract class Pessoa : INotification
{
     public Guid Id { get; set; } 
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Email { get; set; }
    public DateTime DataNascimento { get; set; }

    protected Pessoa(string nome, string telefone, string email, DateTime dataNascimento)
    {
        Nome = nome;
        Telefone = telefone;
        Email = email;
        DataNascimento = dataNascimento;
    }

    public virtual void Notificar(Pessoa pessoa)
    {
        Console.WriteLine($"Nome: {Nome} - Telefone: {Telefone} - E-mail: {Email} - Data de Nascimento: {DataNascimento:dd/MM/yyyy}");
    }
}