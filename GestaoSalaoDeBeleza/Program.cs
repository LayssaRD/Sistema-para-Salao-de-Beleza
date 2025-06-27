using GestaoSalaoDeBeleza.Data;
using GestaoSalaoDeBeleza.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new SalaoContext();
            await context.Database.EnsureCreatedAsync();

            Console.WriteLine("=== Sistema de Gestão do Salão de Beleza ===\n");

            bool continuar = true;
            while (continuar)
            {
                MostrarMenu();
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":   await CadastrarCliente(context);
                                break;
                    case "2":   await ListarClientes(context);
                                break;
                    case "3":   await CriarAgendamento(context);
                                break;
                    case "4":   await ListarAgendamentos(context);
                                break;
                    case "5":   await ListarServicos(context);
                                break;
                    case "6":   await ListarProfissionais(context);
                                break;
                    case "0":   continuar = false;
                                break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void MostrarMenu()
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Cadastrar Cliente");
            Console.WriteLine("2 - Listar Clientes");
            Console.WriteLine("3 - Criar Agendamento");
            Console.WriteLine("4 - Listar Agendamentos");
            Console.WriteLine("5 - Listar Serviços");
            Console.WriteLine("6 - Listar Profissionais");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");
        }

        static async Task CadastrarCliente(SalaoContext context)
        {
            Console.WriteLine("\n=== Cadastrar Cliente ===");

            var cliente = new Cliente();

            Console.Write("Nome: ");
            cliente.Nome = Console.ReadLine() ?? "";

            Console.Write("Telefone: ");
            cliente.Telefone = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            cliente.Email = Console.ReadLine() ?? "";

            Console.Write("Data de Nascimento (dd/mm/aaaa): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dataNasc))
                cliente.DataNascimento = dataNasc;

            Console.Write("Endereço: ");
            cliente.Endereco = Console.ReadLine() ?? "";

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            Console.WriteLine("Cliente cadastrado com sucesso!");
        }

        static async Task ListarClientes(SalaoContext context)
        {
            Console.WriteLine("\n=== Lista de Clientes ===");

            var clientes = await context.Clientes.ToListAsync();

            if (!clientes.Any())
            {
                Console.WriteLine("Nenhum cliente cadastrado.");
                return;
            }

            foreach (var cliente in clientes)
            {
                Console.WriteLine($"ID: {cliente.Id} | Nome: {cliente.Nome} | Telefone: {cliente.Telefone} | Idade: {cliente.CalcularIdade()} anos");
            }
        }

        static async Task CriarAgendamento(SalaoContext context)
        {
            Console.WriteLine("\n=== Criar Agendamento ===");

            var clientes = await context.Clientes.ToListAsync();
            if (!clientes.Any())
            {
                Console.WriteLine("Nenhum cliente cadastrado. Cadastre um cliente primeiro.");
                return;
            }

            Console.WriteLine("Clientes disponíveis:");
            foreach (var c in clientes)
                Console.WriteLine($"{c.Id} - {c.Nome}");

            Console.Write("ID do Cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int clienteId))
                return;

            var profissionais = await context.Profissionais.ToListAsync();
            Console.WriteLine("\nProfissionais disponíveis:");
            foreach (var p in profissionais)
                Console.WriteLine($"{p.Id} - {p.Nome} ({p.Especialidade})");

            Console.Write("ID do Profissional: ");
            if (!int.TryParse(Console.ReadLine(), out int profissionalId))
                return;

            var servicos = await context.Servicos.ToListAsync();
            Console.WriteLine("\nServiços disponíveis:");
            foreach (var s in servicos)
                Console.WriteLine($"{s.ServicoId} - {s.Nome} - R$ {s.Preco:F2}");

            Console.Write("ID do Serviço: ");
            if (!int.TryParse(Console.ReadLine(), out int servicoId))
                return;

            Console.Write("Data do agendamento (dd/MM/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime data))
            {
                Console.WriteLine("Data inválida.");
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

            Console.WriteLine("Agendamento criado com sucesso!");
        }

        static async Task ListarAgendamentos(SalaoContext context)
        {
            Console.WriteLine("\n=== Lista de Agendamentos ===");

            var agendamentos = await context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Profissional)
                .Include(a => a.Servico)
                .OrderBy(a => a.Data)
                .ToListAsync();

            agendamentos = agendamentos.OrderBy(a => a.Data).ThenBy(a => a.Hora).ToList();

            if (!agendamentos.Any())
            {
                Console.WriteLine("Nenhum agendamento encontrado.");
                return;
            }

            foreach (var agendamento in agendamentos)
            {
                var dataHoraCompleta = agendamento.Data.Date + agendamento.Hora;
                Console.WriteLine($"ID: {agendamento.AgendamentoId} | Data e Hora: {dataHoraCompleta:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Cliente: {agendamento.Cliente.Nome} | Profissional: {agendamento.Profissional.Nome}");
                Console.WriteLine($"Serviço: {agendamento.Servico.Nome} - R$ {agendamento.Servico.Preco:F2}");
                Console.WriteLine($"Status: {agendamento.Status}");
                Console.WriteLine(new string('-', 50));
            }
        }

        static async Task ListarServicos(SalaoContext context)
        {
            Console.WriteLine("\n=== Lista de Serviços ===");

            var servicos = await context.Servicos.ToListAsync();

            foreach (var servico in servicos)
            {
                Console.WriteLine($"ID: {servico.ServicoId} | Nome: {servico.Nome}");
                Console.WriteLine($"Preço: R$ {servico.Preco:F2} | Duração: {servico.DuracaoMinutos} min");
                Console.WriteLine($"Descrição: {servico.Descricao}");
                Console.WriteLine(new string('-', 40));
            }
        }

        static async Task ListarProfissionais(SalaoContext context)
        {
            Console.WriteLine("\n=== Lista de Profissionais ===");

            var profissionais = await context.Profissionais.ToListAsync();

            foreach (var profissional in profissionais)
            {
                Console.WriteLine($"ID: {profissional.Id} | Nome: {profissional.Nome}");
                Console.WriteLine($"Especialidade: {profissional.Especialidade}");
                Console.WriteLine($"Telefone: {profissional.Telefone}");
                Console.WriteLine($"Horário: {profissional.HoraInicioTrabalho:hh\\:mm} às {profissional.HoraFimTrabalho:hh\\:mm}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }
}
