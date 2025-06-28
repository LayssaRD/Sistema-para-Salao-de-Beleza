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
                    case "1":
                        await CadastrarCliente(context);
                        break;
                    case "2":
                        await ListarClientes(context);
                        break;
                    case "3":
                        await EditarCliente(context);
                        break;
                    case "4":
                        await ExcluirCliente(context);
                        break;
                    case "5":
                        await CadastrarProfissional(context);
                        break;
                    case "6":
                        await ListarProfissionais(context);
                        break;
                    case "7":
                        await EditarProfissional(context);
                        break;
                    case "8":
                        await ExcluirProfissional(context);
                        break;
                    case "9":
                        await CadastrarServico(context);
                        break;
                    case "10":
                        await ListarServicos(context);
                        break;
                    case "11":
                        await EditarServico(context);
                        break;
                    case "12":
                        await ExcluirServico(context);
                        break;
                    case "13":
                        await CriarAgendamento(context);
                        break;
                    case "14":
                        await ListarAgendamentos(context);
                        break;
                    case "15":
                        await EditarAgendamento(context);
                        break;
                    case "16":
                        break;
                    case "0":
                        continuar = false;
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
            Console.WriteLine("3 - Editar Cliente");
            Console.WriteLine("4 - Excluir Cliente");
            Console.WriteLine("5 - Cadastrar Profissional");
            Console.WriteLine("6 - Listar Profissionais");
            Console.WriteLine("7 - Editar Profissional");
            Console.WriteLine("8 - Excluir Profissional");
            Console.WriteLine("9 - Cadastrar Serviço");
            Console.WriteLine("10 - Listar Servicos");
            Console.WriteLine("11 - Editar Servico");
            Console.WriteLine("12 - Excluir Servico");
            Console.WriteLine("13 - Criar Agendamento");
            Console.WriteLine("14 - Listar Agendamentos");
            Console.WriteLine("15 - Editar Agendamento");
            Console.WriteLine("16 - Excluir Agendamento");
            Console.WriteLine("17 - Cancelar Agendamento");
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
                Console.WriteLine(cliente);
                Console.WriteLine("\n----------------------------------\n");
            }
        }

        static async Task EditarCliente(SalaoContext context)
        {
            Console.WriteLine("\n=== Editar Cliente ===");

            await ListarClientes(context);

            Console.Write("\nDigite o ID do cliente a ser editado: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var cliente = await context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                Console.WriteLine("Cliente não encontrado.");
                return;
            }

            Console.WriteLine($"\nCliente atual:\n{cliente}\n");

            bool continuar = true;
            while (continuar)
            {
                Console.WriteLine("Qual campo deseja editar?");
                Console.WriteLine("1 - Nome");
                Console.WriteLine("2 - Telefone");
                Console.WriteLine("3 - Email");
                Console.WriteLine("4 - Endereço");
                Console.WriteLine("0 - Sair");

                Console.Write("Opção: ");
                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.Write("Novo nome: ");
                        string? nome = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(nome))
                            cliente.Nome = nome;
                        break;

                    case "2":
                        Console.Write("Novo telefone: ");
                        string? telefone = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(telefone))
                            cliente.Telefone = telefone;
                        break;

                    case "3":
                        Console.Write("Novo email: ");
                        string? email = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(email))
                            cliente.Email = email;
                        break;

                    case "4":
                        Console.Write("Novo endereço: ");
                        string? endereco = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(endereco))
                            cliente.Endereco = endereco;
                        break;

                    case "0":
                        continuar = false;
                        break;

                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Alterações salvas com sucesso!");
        }

        static async Task ExcluirCliente(SalaoContext context)
        {
            Console.WriteLine("\n=== Excluir Cliente ===");

            await ListarClientes(context);

            Console.Write("\nDigite o ID do cliente a ser excluído: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var cliente = await context.Clientes
                .Include(c => c.HistoricoAgendamentos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                Console.WriteLine("Cliente não encontrado.");
                return;
            }

            if (cliente.HistoricoAgendamentos.Any())
            {
                Console.WriteLine("Não é possível excluir este cliente. Existem agendamentos associados a ele.");
                return;
            }

            Console.WriteLine($"\nCliente encontrado:\n{cliente}");
            Console.Write("Tem certeza que deseja excluir este cliente? (s/n): ");
            string? confirmacao = Console.ReadLine()?.ToLower();

            if (confirmacao == "s")
            {
                context.Clientes.Remove(cliente);
                await context.SaveChangesAsync();
                Console.WriteLine("Cliente excluído com sucesso!");
            }
            else
            {
                Console.WriteLine("Exclusão cancelada.");
            }
        }

        static async Task CadastrarProfissional(SalaoContext context)
        {
            Console.WriteLine("\n=== Cadastrar Profissional ===");

            var profissional = new Profissional();

            Console.Write("Nome: ");
            profissional.Nome = Console.ReadLine() ?? "";

            Console.Write("Telefone: ");
            profissional.Telefone = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            profissional.Email = Console.ReadLine() ?? "";

            Console.Write("Data de Nascimento (dd/mm/aaaa): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dataNasc))
                profissional.DataNascimento = dataNasc;

            Console.Write("Especialidade: ");
            profissional.Especialidade = Console.ReadLine() ?? "";

            Console.WriteLine("Categorias disponíveis:");
            var categorias = Enum.GetValues<CategoriaServico>();
            for (int i = 0; i < categorias.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {categorias[i]}");
            }

            Console.Write("Selecione a categoria (número): ");
            if (int.TryParse(Console.ReadLine(), out int categoriaSelecionada) &&
                categoriaSelecionada > 0 && categoriaSelecionada <= categorias.Length)
            {
                profissional.Categoria = categorias[categoriaSelecionada - 1];
            }

            Console.Write("Hora início trabalho (HH:mm): ");
            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan horaInicio))
                profissional.HoraInicioTrabalho = horaInicio;

            Console.Write("Hora fim trabalho (HH:mm): ");
            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan horaFim))
                profissional.HoraFimTrabalho = horaFim;

            context.Profissionais.Add(profissional);
            await context.SaveChangesAsync();

            Console.WriteLine("Profissional cadastrado com sucesso!");
        }

        static async Task ListarProfissionais(SalaoContext context)
        {
            Console.WriteLine("\n=== Lista de Profissionais ===");

            var profissionais = await context.Profissionais.ToListAsync();

            foreach (var profissional in profissionais)
            {
                Console.WriteLine(profissional);
                Console.WriteLine("\n----------------------------------\n");

            }
        }

        static async Task EditarProfissional(SalaoContext context)
        {
            Console.WriteLine("\n=== Editar Profissional ===");

            await ListarProfissionais(context);

            Console.Write("\nDigite o ID do profissional a ser editado: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var profissional = await context.Profissionais.FindAsync(id);
            if (profissional == null)
            {
                Console.WriteLine("Profissional não encontrado.");
                return;
            }

            bool continuar = true;
            while (continuar)
            {
                Console.WriteLine($"\nProfissional atual:\n{profissional}");
                Console.WriteLine("\nQual campo deseja editar?");
                Console.WriteLine("1 - Nome");
                Console.WriteLine("2 - Telefone");
                Console.WriteLine("3 - Email");
                Console.WriteLine("4 - Especialidade");
                Console.WriteLine("5 - Categoria");
                Console.WriteLine("6 - Hora Início de Trabalho");
                Console.WriteLine("7 - Hora Fim de Trabalho");
                Console.WriteLine("0 - Sair");

                Console.Write("Opção: ");
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.Write("Novo nome: ");
                        var nome = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(nome))
                            profissional.Nome = nome;
                        break;

                    case "2":
                        Console.Write("Novo telefone: ");
                        var telefone = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(telefone))
                            profissional.Telefone = telefone;
                        break;

                    case "3":
                        Console.Write("Novo email: ");
                        var email = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(email))
                            profissional.Email = email;
                        break;

                    case "4":
                        Console.Write("Nova especialidade: ");
                        var especialidade = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(especialidade))
                            profissional.Especialidade = especialidade;
                        break;

                    case "5":
                        Console.WriteLine("Categorias disponíveis:");
                        var categorias = Enum.GetValues<CategoriaServico>();
                        for (int i = 0; i < categorias.Length; i++)
                        {
                            Console.WriteLine($"{i + 1} - {categorias[i]}");
                        }

                        Console.Write("Digite o número da nova categoria: ");
                        var entrada = Console.ReadLine();

                        if (int.TryParse(entrada, out int categoriaSelecionada) &&
                            categoriaSelecionada > 0 && categoriaSelecionada <= categorias.Length)
                        {
                            profissional.Categoria = categorias[categoriaSelecionada - 1];
                        }
                        else
                        {
                            Console.WriteLine("Categoria inválida.");
                        }
                        break;

                    case "6":
                        Console.Write("Nova hora de início (hh:mm): ");
                        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan horaInicio))
                            profissional.HoraInicioTrabalho = horaInicio;
                        else
                            Console.WriteLine("Hora inválida.");
                        break;

                    case "7":
                        Console.Write("Nova hora de fim (hh:mm): ");
                        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan horaFim))
                            profissional.HoraFimTrabalho = horaFim;
                        else
                            Console.WriteLine("Hora inválida.");
                        break;

                    case "0":
                        continuar = false;
                        break;

                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Profissional editado com sucesso!");
        }

        static async Task ExcluirProfissional(SalaoContext context)
        {
            Console.WriteLine("\n=== Excluir Profissional ===");

            await ListarProfissionais(context);

            Console.Write("\nDigite o ID do profissional a ser excluído: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var profissional = await context.Profissionais
                .Include(p => p.Agendamentos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profissional == null)
            {
                Console.WriteLine("Profissional não encontrado.");
                return;
            }

            if (profissional.Agendamentos.Any())
            {
                Console.WriteLine("Não é possível excluir este profissional. Existem agendamentos associados a ele.");
                return;
            }

            Console.WriteLine($"\nProfissional encontrado:\n{profissional}");
            Console.Write("Tem certeza que deseja excluir este profissional? (s/n): ");
            var confirmacao = Console.ReadLine()?.ToLower();

            if (confirmacao == "s")
            {
                context.Profissionais.Remove(profissional);
                await context.SaveChangesAsync();
                Console.WriteLine("Profissional excluído com sucesso!");
            }
            else
            {
                Console.WriteLine("Exclusão cancelada.");
            }
        }

        static async Task CadastrarServico(SalaoContext context)
        {
            Console.WriteLine("\n=== Cadastrar Novo Serviço ===");

            Console.Write("Nome do Serviço: ");
            var nome = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Nome inválido.");
                return;
            }

            Console.Write("Preço (ex: 25.50): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal preco))
            {
                Console.WriteLine("Preço inválido.");
                return;
            }

            Console.Write("Duração (em minutos): ");
            if (!int.TryParse(Console.ReadLine(), out int duracao))
            {
                Console.WriteLine("Duração inválida.");
                return;
            }

            Console.WriteLine("Categorias disponíveis:");
            foreach (var cat in Enum.GetValues(typeof(CategoriaServico)))
                Console.WriteLine($"{(int)cat} - {cat}");

            Console.Write("Selecione o número da categoria: ");
            if (!int.TryParse(Console.ReadLine(), out int categoriaSelecionada) || !Enum.IsDefined(typeof(CategoriaServico), categoriaSelecionada))
            {
                Console.WriteLine("Categoria inválida.");
                return;
            }

            var servico = new ServicoOfertado
            {
                Nome = nome,
                Preco = preco,
                DuracaoMinutos = duracao,
                Categoria = (CategoriaServico)categoriaSelecionada
            };

            await context.Servicos.AddAsync(servico);
            await context.SaveChangesAsync();

            Console.WriteLine("Serviço cadastrado com sucesso!");
        }

        static async Task ListarServicos(SalaoContext context)
        {
            Console.WriteLine("\n=== Lista de Serviços ===");

            var servicos = await context.Servicos.ToListAsync();

            foreach (var servico in servicos)
            {
                Console.WriteLine(servico);
                Console.WriteLine("\n----------------------------------\n");
            }
        }

        static async Task EditarServico(SalaoContext context)
        {
            Console.WriteLine("\n=== Editar Serviço ===");

            await ListarServicos(context);

            Console.Write("\nDigite o ID do serviço a ser editado: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var servico = await context.Servicos.FindAsync(id);
            if (servico == null)
            {
                Console.WriteLine("Serviço não encontrado.");
                return;
            }

            bool continuar = true;
            while (continuar)
            {
                Console.WriteLine($"\nServiço atual:\n{servico}");
                Console.WriteLine("\nQual campo deseja editar?");
                Console.WriteLine("1 - Nome");
                Console.WriteLine("2 - Preço");
                Console.WriteLine("3 - Duração (em minutos)");
                Console.WriteLine("4 - Categoria");
                Console.WriteLine("0 - Sair");

                Console.Write("Opção: ");
                string? opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.Write("Novo nome: ");
                        var nome = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(nome))
                            servico.Nome = nome;
                        break;

                    case "2":
                        Console.Write("Novo preço: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal preco))
                            servico.Preco = preco;
                        else
                            Console.WriteLine("Preço inválido.");
                        break;

                    case "3":
                        Console.Write("Nova duração (em minutos): ");
                        if (int.TryParse(Console.ReadLine(), out int duracao))
                            servico.DuracaoMinutos = duracao;
                        else
                            Console.WriteLine("Duração inválida.");
                        break;

                    case "4":
                        Console.WriteLine("Categorias disponíveis:");
                        var categorias = Enum.GetValues<CategoriaServico>();
                        for (int i = 0; i < categorias.Length; i++)
                        {
                            Console.WriteLine($"{i + 1} - {categorias[i]}");
                        }

                        Console.Write("Digite o número da nova categoria: ");
                        var entrada = Console.ReadLine();
                        if (int.TryParse(entrada, out int selecionado) &&
                            selecionado > 0 && selecionado <= categorias.Length)
                        {
                            servico.Categoria = categorias[selecionado - 1];
                        }
                        else
                        {
                            Console.WriteLine("Categoria inválida.");
                        }
                        break;

                    case "0":
                        continuar = false;
                        break;

                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Serviço editado com sucesso!");
        }

        static async Task ExcluirServico(SalaoContext context)
        {
            Console.WriteLine("\n=== Excluir Serviço ===");

            await ListarServicos(context);

            Console.Write("\nDigite o ID do serviço a ser excluído: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var servico = await context.Servicos
                .Include(s => s.Agendamentos)
                .FirstOrDefaultAsync(s => s.ServicoId == id);

            if (servico == null)
            {
                Console.WriteLine("Serviço não encontrado.");
                return;
            }

            if (servico.Agendamentos.Any())
            {
                Console.WriteLine("Não é possível excluir este serviço. Existem agendamentos associados a ele.");
                return;
            }

            Console.WriteLine($"\nServiço encontrado:\n{servico}");
            Console.Write("Tem certeza que deseja excluir este serviço? (s/n): ");
            var confirmacao = Console.ReadLine()?.ToLower();

            if (confirmacao == "s")
            {
                context.Servicos.Remove(servico);
                await context.SaveChangesAsync();
                Console.WriteLine("Serviço excluído com sucesso!");
            }
            else
            {
                Console.WriteLine("Exclusão cancelada.");
            }
        }

        static async Task CriarAgendamento(SalaoContext context)
        {
            Console.WriteLine("\n=== Criar Agendamento ===");

            var clientes = await context.Clientes.ToListAsync();
            if (!clientes.Any())
            {
                Console.WriteLine("Nenhum cliente cadastrado. Cadastre um cliente primeiro.");
                await CadastrarCliente(context);
                return;
            }

            Console.WriteLine("Clientes disponíveis:");
            foreach (var c in clientes)
                Console.WriteLine($"{c.Id} - {c.Nome}");

            Console.Write("ID do Cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int clienteId))
                return;

            var servicos = await context.Servicos.ToListAsync();
            if (!servicos.Any())
            {
                Console.WriteLine("Nenhum serviço cadastrado. Cadastre um serviço primeiro.");
                await CadastrarServico(context);
                return;
            }
            Console.WriteLine("\nServiços disponíveis:");
            foreach (var s in servicos)
                Console.WriteLine($"{s.ServicoId} - {s.Nome} (Categoria: {s.Categoria}) - R$ {s.Preco:F2} - {s.DuracaoMinutos} min");

            Console.Write("ID do Serviço: ");
            if (!int.TryParse(Console.ReadLine(), out int servicoId))
                return;

            var servicoSelecionado = await context.Servicos.FindAsync(servicoId);
            if (servicoSelecionado == null)
            {
                Console.WriteLine("Serviço não encontrado.");
                return;
            }

            var profissionaisDisponiveis = await context.Profissionais
               .Where(p => p.Categoria == servicoSelecionado.Categoria)
               .ToListAsync();

            if (!profissionaisDisponiveis.Any())
            {
                Console.WriteLine($"Nenhum profissional disponível para a categoria {servicoSelecionado.Categoria}.");
                return;
            }

            Console.WriteLine("\nProfissionais disponíveis para este serviço:");
            foreach (var p in profissionaisDisponiveis)
                Console.WriteLine($"{p.Id} - {p.Nome} ({p.Especialidade})");

            Console.Write("ID do Profissional: ");
            if (!int.TryParse(Console.ReadLine(), out int profissionalId))
                return;

            var profissionalSelecionado = profissionaisDisponiveis.FirstOrDefault(p => p.Id == profissionalId);
            if (profissionalSelecionado == null)
            {
                Console.WriteLine("Profissional não encontrado ou não pode fazer este serviço.");
                return;
            }

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

            var dataHoraAgendamento = data.Date + hora;

            if (!profissionalSelecionado.EstaDisponivelNoHorario(dataHoraAgendamento, servicoSelecionado.DuracaoMinutos, context))
            {
                Console.WriteLine("Profissional não está disponível neste horário.");
                Console.WriteLine("Verifique:");
                Console.WriteLine($"- Horário de trabalho: {profissionalSelecionado.HoraInicioTrabalho:hh\\:mm} às {profissionalSelecionado.HoraFimTrabalho:hh\\:mm}");
                Console.WriteLine("- Conflitos com outros agendamentos");
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
            Console.WriteLine($"Serviço: {servicoSelecionado.Nome}");
            Console.WriteLine($"Duração: {servicoSelecionado.DuracaoMinutos} minutos");
            Console.WriteLine($"Término previsto: {dataHoraAgendamento.AddMinutes(servicoSelecionado.DuracaoMinutos):dd/MM/yyyy HH:mm}");
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

        static async Task EditarAgendamento(SalaoContext context)
        {
            Console.WriteLine("\n=== Editar Agendamento ===");

            var agendamentos = await context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Profissional)
                .Include(a => a.Servico)
                .ToListAsync();

            if (!agendamentos.Any())
            {
                Console.WriteLine("Nenhum agendamento encontrado.");
                return;
            }

            Console.WriteLine("Agendamentos disponíveis:");

            await ListarAgendamentos(context);

            Console.Write("Digite o ID do agendamento que deseja editar: ");
            if (!Guid.TryParse(Console.ReadLine(), out Guid agendamentoId))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var agendamento = await context.Agendamentos.FindAsync(agendamentoId);
            if (agendamento == null)
            {
                Console.WriteLine("Agendamento não encontrado.");
                return;
            }

            Console.Write("Nova data (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime novaData))
            {
                Console.WriteLine("Data inválida.");
                return;
            }

            Console.Write("Nova hora (hh:mm): ");
            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan novaHora))
            {
                Console.WriteLine("Hora inválida.");
                return;
            }

            await ListarClientes(context);

            Console.Write("ID do novo Cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int novoClienteId))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            await ListarProfissionais(context);

            Console.Write("ID do novo Profissional: ");
            if (!int.TryParse(Console.ReadLine(), out int novoProfissionalId))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            await ListarServicos(context);

            Console.Write("ID do novo Serviço: ");
            if (!int.TryParse(Console.ReadLine(), out int novoServicoId))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            agendamento.Data = novaData;
            agendamento.Hora = novaHora;
            agendamento.ClienteId = novoClienteId;
            agendamento.ProfissionalId = novoProfissionalId;
            agendamento.ServicoId = novoServicoId;

            context.Agendamentos.Update(agendamento);
            await context.SaveChangesAsync();

            Console.WriteLine("Agendamento atualizado com sucesso!");
        }

    }
}
