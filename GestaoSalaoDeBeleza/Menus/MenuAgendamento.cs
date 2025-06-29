
using GestaoSalaoDeBeleza.Data;
using GestaoSalaoDeBeleza.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza.Menus;

public class MenuAgendamento
{
    public static async Task ExibirMenu(SalaoContext context)
    {
        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("=== MENU AGENDAMENTOS ===\n");
            Console.WriteLine("1 - Criar Agendamento");
            Console.WriteLine("2 - Listar Agendamentos");
            Console.WriteLine("3 - Editar Agendamento");
            Console.WriteLine("4 - Excluir Agendamento");
            Console.WriteLine("0 - Voltar ao Menu Principal");
            Console.Write("\nOpção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await CriarAgendamento(context);
                    break;
                case "2":
                    await ListarAgendamentos(context);
                    break;
                case "3":
                    await EditarAgendamento(context);
                    break;
                case "4":
                    await ExcluirAgendamento(context);
                    break;
                case "0":
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

            if (continuar && opcao != "0")
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    private static async Task CriarAgendamento(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Criar Agendamento ===\n");

        var clientes = await context.Clientes.ToListAsync();
        if (!clientes.Any())
        {
            Console.WriteLine("Nenhum cliente cadastrado. Cadastre um cliente primeiro.");
            return;
        }

        Console.WriteLine("Clientes disponíveis:");
        foreach (var c in clientes)
        {
            Console.WriteLine($"ID: {c.Id} - {c.Nome} - {c.Telefone}");
        }

        Console.Write("\nID do Cliente: ");
        if (!int.TryParse(Console.ReadLine(), out int clienteId))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        var cliente = await context.Clientes.FindAsync(clienteId);
        if (cliente == null)
        {
            Console.WriteLine("Cliente não encontrado.");
            return;
        }

        var servicos = await context.Servicos.ToListAsync();
        if (!servicos.Any())
        {
            Console.WriteLine("Nenhum serviço cadastrado. Cadastre um serviço primeiro.");
            return;
        }

        Console.WriteLine("\nServiços disponíveis:");
        foreach (var s in servicos)
        {
            Console.WriteLine($"ID: {s.ServicoId} - {s.Nome} - R$ {s.Preco:F2} - {s.DuracaoMinutos}min - {s.Categoria}");
        }

        Console.Write("\nID do Serviço: ");
        if (!int.TryParse(Console.ReadLine(), out int servicoId))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        var servicoSelecionado = await context.Servicos.FindAsync(servicoId);
        if (servicoSelecionado == null)
        {
            Console.WriteLine("Serviço não encontrado.");
            return;
        }

        var profissionaisDaCategoria = await context.Profissionais
            .Where(p => p.Categoria == servicoSelecionado.Categoria)
            .ToListAsync();

        if (!profissionaisDaCategoria.Any())
        {
            Console.WriteLine($"Nenhum profissional disponível para a categoria {servicoSelecionado.Categoria}.");
            return;
        }

        Console.WriteLine($"\nProfissionais disponíveis para {servicoSelecionado.Categoria}:");
        foreach (var p in profissionaisDaCategoria)
        {
            Console.WriteLine($"ID: {p.Id} - {p.Nome} - {p.Especialidade}");
            Console.WriteLine($"   Horário: {p.HoraInicioTrabalho:hh\\:mm} às {p.HoraFimTrabalho:hh\\:mm}");
        }

        Console.Write("\nID do Profissional: ");
        if (!int.TryParse(Console.ReadLine(), out int profissionalId))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        var profissionalSelecionado = profissionaisDaCategoria.FirstOrDefault(p => p.Id == profissionalId);
        if (profissionalSelecionado == null)
        {
            Console.WriteLine("Profissional não encontrado ou não pertence à categoria do serviço.");
            return;
        }

        Console.Write("\nData do agendamento (dd/MM/yyyy): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out DateTime data))
        {
            Console.WriteLine("Data inválida.");
            return;
        }

        if (data.Date <= DateTime.Today)
        {
            Console.WriteLine("A data do agendamento deve ser maior que hoje.");
            return;
        }

        Console.Write("Hora do agendamento (HH:mm): ");
        if (!TimeSpan.TryParseExact(Console.ReadLine(), @"hh\:mm",
            System.Globalization.CultureInfo.InvariantCulture,
            out TimeSpan hora))
        {
            Console.WriteLine("Hora inválida.");
            return;
        }

        var dataHoraAgendamento = data.Date + hora;

        if (!profissionalSelecionado.EstaDisponivelNoHorario(dataHoraAgendamento, servicoSelecionado.DuracaoMinutos, context))
        {
            Console.WriteLine("\nProfissional não está disponível neste horário!");
            Console.WriteLine("Verifique:");
            Console.WriteLine($"- Horário de trabalho: {profissionalSelecionado.HoraInicioTrabalho:hh\\:mm} às {profissionalSelecionado.HoraFimTrabalho:hh\\:mm}");
            Console.WriteLine("- Conflitos com outros agendamentos existentes.");
            return;
        }

        var agendamento = new Agendamento
        {
            ClienteId = clienteId,
            ProfissionalId = profissionalId,
            ServicoId = servicoId,
            Data = data,
            Hora = hora
        };

        context.Agendamentos.Add(agendamento);
        await context.SaveChangesAsync();

        var agendamentoCompleto = await context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .FirstOrDefaultAsync(a => a.AgendamentoId == agendamento.AgendamentoId);

        Console.WriteLine("\nAgendamento criado com sucesso!");
        Console.WriteLine($"\n{agendamentoCompleto}");
    }

    private static async Task ListarAgendamentos(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Lista de Agendamentos ===\n");

        var agendamentos = await context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .ToListAsync();

        agendamentos = agendamentos
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Hora)
            .ToList();

        foreach (var agendamento in agendamentos)
        {
            Console.WriteLine(agendamento);
            Console.WriteLine("\n----------------------------------\n");
        }
    }

    private static async Task EditarAgendamento(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Editar Agendamento ===\n");

        await ListarAgendamentos(context);

        Console.Write("\nDigite o ID do agendamento que deseja editar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        var agendamento = await context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .FirstOrDefaultAsync(a => a.AgendamentoId == id);

        if (agendamento == null)
        {
            Console.WriteLine("Agendamento não encontrado.");
            return;
        }

        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("=== Editar Agendamento ===\n");
            Console.WriteLine($"Agendamento atual:\n{agendamento}\n");

            Console.WriteLine("Qual campo deseja editar?");
            Console.WriteLine("1 - Data");
            Console.WriteLine("2 - Hora");
            Console.WriteLine("3 - Cliente");
            Console.WriteLine("4 - Profissional");
            Console.WriteLine("5 - Serviço");
            Console.WriteLine("6 - Alterar Status");
            Console.WriteLine("0 - Salvar e Sair");
            Console.Write("\nOpção: ");
            
            string? opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("Nova data (dd/MM/yyyy): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime novaData))
                    {
                        if (novaData.Date <= DateTime.Today)
                            Console.WriteLine("A data do agendamento deve ser maior que hoje.");
                        else
                            agendamento.Data = novaData;
                    }
                    else
                    {
                        Console.WriteLine("Data inválida.");
                    }

                    break;

                case "2":
                    Console.Write("Nova hora (HH:mm): ");
                    if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan novaHora))
                        agendamento.Hora = novaHora;
                    else
                        Console.WriteLine("Hora inválida.");
                    break;

                case "3":
                    var clientes = await context.Clientes.ToListAsync();
                    Console.WriteLine("\nClientes disponíveis:");
                    foreach (var c in clientes)
                    {
                        Console.WriteLine($"ID: {c.Id} - {c.Nome}");
                    }
                    Console.Write("\nID do novo Cliente: ");
                    if (int.TryParse(Console.ReadLine(), out int novoClienteId))
                        agendamento.ClienteId = novoClienteId;
                    else
                        Console.WriteLine("ID inválido.");
                    break;

                case "4":
                    var profissionais = await context.Profissionais
                        .Where(p => p.Categoria == agendamento.Servico.Categoria)
                        .ToListAsync();
                    Console.WriteLine($"\nProfissionais disponíveis para {agendamento.Servico.Categoria}:");
                    foreach (var p in profissionais)
                    {
                        Console.WriteLine($"ID: {p.Id} - {p.Nome}");
                    }
                    Console.Write("\nID do novo Profissional: ");
                    if (int.TryParse(Console.ReadLine(), out int novoProfissionalId))
                    {
                        var novoProfissional = await context.Profissionais.FindAsync(novoProfissionalId);
                        var novaDataHora = agendamento.Data.Date + agendamento.Hora;
                        if (novoProfissional != null &&
                            novoProfissional.Categoria == agendamento.Servico.Categoria &&
                            novoProfissional.EstaDisponivelNoHorario(novaDataHora, agendamento.Servico.DuracaoMinutos, context))
                        {
                            agendamento.ProfissionalId = novoProfissionalId;
                        }
                        else
                        {
                            Console.WriteLine("Profissional inválido ou indisponível no horário.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;

                case "5":
                    var servicos = await context.Servicos.ToListAsync();
                    Console.WriteLine("\nServiços disponíveis:");
                    foreach (var s in servicos)
                    {
                        Console.WriteLine($"ID: {s.ServicoId} - {s.Nome} - {s.Categoria}");
                    }
                    Console.Write("\nID do novo Serviço: ");
                    if (int.TryParse(Console.ReadLine(), out int novoServicoId))
                    {
                        var novoServico = await context.Servicos.FindAsync(novoServicoId);
                        var novoProfissional = await context.Profissionais.FindAsync(agendamento.ProfissionalId);
                        var novaDataHora = agendamento.Data.Date + agendamento.Hora;

                        if (novoServico != null &&
                            novoProfissional != null &&
                            novoProfissional.Categoria == novoServico.Categoria &&
                            novoProfissional.EstaDisponivelNoHorario(novaDataHora, novoServico.DuracaoMinutos, context))
                        {
                            agendamento.ServicoId = novoServicoId;
                        }
                        else
                        {
                            Console.WriteLine("Serviço incompatível ou horário inválido.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;

                case "6":
                    Console.WriteLine("\nEscolha o novo status:");
                    Console.WriteLine("1 - Agendado");
                    Console.WriteLine("2 - Concluído");
                    Console.WriteLine("3 - Cancelado");
                    Console.Write("Opção: ");
                    string? statusOp = Console.ReadLine();

                    try
                    {
                        switch (statusOp)
                        {
                            case "1":
                                agendamento.Status = StatusAgendamento.Agendado;
                                break;
                            case "2":
                                agendamento.MarcarComoConcluido();
                                break;
                            case "3":
                                agendamento.Cancelar();
                                break;
                            default:
                                Console.WriteLine("Opção de status inválida.");
                                break;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Erro ao alterar status: {ex.Message}");
                    }
                    break;

                case "0":
                    continuar = false;
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

            if (continuar && opcao != "0")
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("\nAgendamento editado com sucesso!");
    }

    private static async Task ExcluirAgendamento(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Excluir Agendamento ===\n");

        await ListarAgendamentos(context);

        Console.Write("\nDigite o ID do agendamento que deseja excluir: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        var agendamento = await context.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Profissional)
            .Include(a => a.Servico)
            .FirstOrDefaultAsync(a => a.AgendamentoId == id);

        if (agendamento == null)
        {
            Console.WriteLine("Agendamento não encontrado.");
            return;
        }

        Console.WriteLine($"\nAgendamento encontrado:\n{agendamento}");
        Console.Write("\nTem certeza que deseja excluir este agendamento? (S/N): ");
        var confirmacao = Console.ReadLine();
        
        if (confirmacao == null || !confirmacao.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Exclusão cancelada.");
            return;
        }

        context.Agendamentos.Remove(agendamento);
        await context.SaveChangesAsync();

        Console.WriteLine("Agendamento excluído com sucesso!");
    }
}
