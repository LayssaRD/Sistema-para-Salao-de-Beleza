namespace GestaoSalaoDeBeleza.Models;

public class Agendamento
{
    public int AgendamentoId { get; set; }
    public DateTime Data { get; set; }
    public TimeSpan Hora { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
    public int ClienteId { get; set; }
    public int ProfissionalId { get; set; }
    public int ServicoId { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Profissional Profissional { get; set; } = null!;
    public ServicoOfertado Servico { get; set; } = null!;

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
        var dataHoraInicio = Data.Date + Hora;
        var fim = Servico != null ? dataHoraInicio.AddMinutes(Servico.DuracaoMinutos) : dataHoraInicio;

        return $"ID: {AgendamentoId} - Agendamento em {dataHoraInicio:dd/MM/yyyy HH:mm} até {fim:HH:mm}\n" +
            $"Cliente: {Cliente?.Nome}\n Profissional: {Profissional?.Nome}\n" +
            $"Serviço: {Servico?.Nome} ({Servico?.DuracaoMinutos} min) - Status: {Status}";
    }

}
public enum StatusAgendamento
{
    Agendado,
    Concluido,
    Cancelado
}
