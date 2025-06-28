using GenericRepository.Entidade;
using GestaoSalaoDeBeleza.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza.Models;

public class Profissional : Pessoa
{
    public string Especialidade { get; set; } = string.Empty;
    public CategoriaServico Categoria { get; set; }
    public TimeSpan HoraInicioTrabalho { get; set; }
    public TimeSpan HoraFimTrabalho { get; set; }
    public List<Agendamento> Agendamentos { get; set; } = new();

    public Profissional() : base() { }

    public Profissional(string nome, string telefone, string email, DateTime dataNascimento)
        : base(nome, telefone, email, dataNascimento)
    {
    }

    public bool EstaDisponivelNoHorario(DateTime dataInicio, int duracaoMinutos, SalaoContext context)
    {
        var dataFim = dataInicio.AddMinutes(duracaoMinutos);
        var horarioInicio = dataInicio.TimeOfDay;
        var horarioFim = dataFim.TimeOfDay;

        if (horarioInicio < HoraInicioTrabalho || horarioFim > HoraFimTrabalho)
            return false;

        var agendamentosNoDia = context.Agendamentos
            .Where(a => a.ProfissionalId == Id && 
                       a.Data.Date == dataInicio.Date && 
                       a.Status == StatusAgendamento.Agendado)
            .Include(a => a.Servico)
            .ToList();

        foreach (var agendamento in agendamentosNoDia)
        {
            var inicioExistente = agendamento.Data.Date + agendamento.Hora;
            var fimExistente = inicioExistente.AddMinutes(agendamento.Servico.DuracaoMinutos);

            if (dataInicio < fimExistente && dataFim > inicioExistente)
                return false;
        }

        return true;
    }

    public bool PodeFazerServico(ServicoOfertado servico)
    {
        return Categoria == servico.Categoria;
    }
    
    public bool HorariosConflitam(TimeSpan inicioNovo, TimeSpan fimNovo)
    {
        return !(fimNovo <= HoraInicioTrabalho || inicioNovo >= HoraFimTrabalho);
    }

    public override string ToString()
    {
        return $"ID: {Id} \nNome: {Nome} \nTelefone: {Telefone} \nEmail: {Email} " +
                   $"\nData Nascimento: {DataNascimento:dd/MM/yyyy} \nEspecialidade: {Especialidade} " +
                   $"\nCategoria: {Categoria} \nHorário: {HoraInicioTrabalho:hh\\:mm} às {HoraFimTrabalho:hh\\:mm}";
    }
}
