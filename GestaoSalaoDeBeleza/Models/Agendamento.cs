namespace GestaoSalaoDeBeleza.Models;

public class Agendamento
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime Data { get; set; }
    public TimeSpan Hora { get; set; }

    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
    public Cliente Cliente { get; set; }
    public Profissional Profissional { get; set; }
    public List<ServicoOfertado> Servicos { get; set; } = new();

    public List<string> ErrosDeValidacao { get; private set; } = new();

    public decimal ValorTotal => Servicos.Sum(s => s.Preco);

    public Agendamento(DateTime data, TimeSpan hora, Cliente cliente, Profissional profissional)
    {
        Data = data.Date;
        Hora = hora;
        Cliente = cliente;
        Profissional = profissional;

        if (!ValidarAgendamento())
        {
            throw new ArgumentException(string.Join("\n", ErrosDeValidacao));
        }

        var dataHoraCompleta = Data.Add(Hora);

        var horarioDia = profissional.HorarioTrabalho.FirstOrDefault(h => true);
        horarioDia?.AdicionarHorarioIndisponivel(dataHoraCompleta);
    }

    public bool ValidarAgendamento()
    {
        ErrosDeValidacao.Clear();

        var dataHoraCompleta = Data.Add(Hora);
        var agora = DateTime.Now;

        if (dataHoraCompleta < agora)
        {
            ErrosDeValidacao.Add("A data e hora do agendamento não podem ser anteriores ao momento atual.");
        }

        var horarioTrabalho = Profissional.HorarioTrabalho.FirstOrDefault();
        if (horarioTrabalho == null)
        {
            ErrosDeValidacao.Add("O profissional não possui horários de trabalho cadastrados.");
            return false;
        }

        if (!horarioTrabalho.EstaDisponivel(dataHoraCompleta))
        {
            ErrosDeValidacao.Add($"O horário {Hora:hh\\:mm} está indisponível para o profissional.");
        }

        return ErrosDeValidacao.Count == 0;
    }

    public void MarcarComoConcluido()
    {
        if (Status != StatusAgendamento.Agendado)
            throw new InvalidOperationException("Somente agendamentos no status 'Agendado' podem ser concluídos.");

        Status = StatusAgendamento.Concluido;
    }

    public void Cancelar()
    {
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Não é possível cancelar um agendamento que já foi concluído.");

        Status = StatusAgendamento.Cancelado;
    }

    public override string ToString()
    {
        var dataHoraCompleta = Data.Add(Hora);
        return $"Agendamento em {dataHoraCompleta:dd/MM/yyyy HH:mm} com {Profissional.Nome} para {Cliente.Nome} - Status: {Status}";
    }

}



public enum StatusAgendamento
{
    Agendado,
    Concluido,
    Cancelado
}
