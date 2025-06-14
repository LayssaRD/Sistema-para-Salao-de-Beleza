using GenericRepository.Entidade;

namespace GestaoSalaoDeBeleza.Models;

public class Profissional : Pessoa, IEntidade
{
    public List<Especialidade> Especialidades { get; set; } = new();
    public List<HorarioTrabalho> HorarioTrabalho { get; set; } = new();

    public Profissional() : base("", "", "", DateTime.MinValue) { }

    public Profissional(string nome, string telefone, string email, DateTime dataNascimento)
        : base(nome, telefone, email, dataNascimento)
    {
    }


    public void AdicionarEspecialidade(Especialidade especialidade)
    {
        if (!Especialidades.Contains(especialidade))
            Especialidades.Add(especialidade);
    }

    public bool HorariosConflitam(TimeSpan inicioNovo, TimeSpan fimNovo)
    {
        foreach (var h in HorarioTrabalho)
        {
            // Se o intervalo novo se sobrepõe a algum existente, retorna true (conflito)
            if (!(fimNovo <= h.HoraInicio || inicioNovo >= h.HoraFim))
                return true;
        }
        return false;
    }

    // Tenta adicionar horário só se não conflitar
    public bool TentarAdicionarHorario(HorarioTrabalho horario)
    {
        if (HorariosConflitam(horario.HoraInicio, horario.HoraFim))
            return false;

        HorarioTrabalho.Add(horario);
        return true;
    }

    public void RemoverEspecialidade(Especialidade especialidade)
    {
        Especialidades.Remove(especialidade);
    }

    public void RemoverHorarioTrabalho(HorarioTrabalho horario)
    {
        HorarioTrabalho.Remove(horario);
    }


    public override string ToString()
    {
        var especialidades = Especialidades.Any()
            ? string.Join(", ", Especialidades)
            : "Nenhuma";

        var horarios = HorarioTrabalho.Any()
            ? string.Join("\n", HorarioTrabalho.Select(h => $"- {h.HoraInicio:hh\\:mm} às {h.HoraFim:hh\\:mm}"))
            : "Nenhum";

        return $"ID: {Id}\nNome: {Nome}\nTelefone: {Telefone}\nEmail: {Email}" +
               $"\nData de Nascimento: {DataNascimento:dd/MM/yyyy}" +
               $"\nEspecialidades: {especialidades}" +
               $"\nHorários de Trabalho:\n{horarios}";
    }

}

public enum Especialidade
{
    Cabelo,
    Unha,
    Estetica,
    Sobrancelha,
    Maquiagem
}
