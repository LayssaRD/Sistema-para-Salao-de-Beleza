using GenericRepository.Entidade;

namespace GestaoSalaoDeBeleza.Models;

public class Profissional : Pessoa
{
    public string Especialidade { get; set; } = string.Empty;
    public TimeSpan HoraInicioTrabalho { get; set; }
    public TimeSpan HoraFimTrabalho { get; set; }
    public List<Agendamento> Agendamentos { get; set; } = new();

    public Profissional() : base() { }

    public Profissional(string nome, string telefone, string email, DateTime dataNascimento)
        : base(nome, telefone, email, dataNascimento)
    {
    }

    public bool EstaDisponivel(DateTime dataHora)
    {
        var horario = dataHora.TimeOfDay;
        return horario >= HoraInicioTrabalho && horario <= HoraFimTrabalho;
    }
    
    public bool HorariosConflitam(TimeSpan inicioNovo, TimeSpan fimNovo)
    {
        return !(fimNovo <= HoraInicioTrabalho || inicioNovo >= HoraFimTrabalho);
    }

    public override string ToString()
    {
        return $"ID: {Id} \nNome: {Nome} \nTelefone: {Telefone} \nEmail: {Email} " +
                   $"\nData Nascimento: {DataNascimento:dd/MM/yyyy} \nEspecialidade: {Especialidade} " +
                   $"\nHorário: {HoraInicioTrabalho:hh\\:mm} às {HoraFimTrabalho:hh\\:mm}";
    }
}
