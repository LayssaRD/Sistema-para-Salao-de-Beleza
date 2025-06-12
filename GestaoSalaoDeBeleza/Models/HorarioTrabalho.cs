using System;

namespace GestaoSalaoDeBeleza.Models;

public class HorarioTrabalho
{
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public List<DateTime> HorariosIndisponiveis { get; set; } = new();

    public HorarioTrabalho(TimeSpan horaInicio, TimeSpan horaFim)
    {
        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }

    public bool EstaDisponivel(DateTime dataHora)
    {
        var horario = dataHora.TimeOfDay;
        return horario >= HoraInicio &&
               horario <= HoraFim &&
               !HorariosIndisponiveis.Contains(dataHora);
    }

    public void AdicionarHorarioIndisponivel(DateTime dataHora)
    {
        if (!HorariosIndisponiveis.Contains(dataHora))
        {
            HorariosIndisponiveis.Add(dataHora);
        }
    }

    public override string ToString()
    {
        return $"Horário Atendimento: {HoraInicio:hh\\:mm} - {HoraFim:hh\\:mm}";
    }
}
