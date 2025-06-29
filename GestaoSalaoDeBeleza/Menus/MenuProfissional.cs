using GestaoSalaoDeBeleza.Data;
using GestaoSalaoDeBeleza.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza.Menus;

public class MenuProfissional
{
    public static async Task ExibirMenu(SalaoContext context)
    {
        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("=== GESTÃO DE PROFISSIONAIS ===\n");
            Console.WriteLine("1 - Cadastrar Profissional");
            Console.WriteLine("2 - Listar Profissionais");
            Console.WriteLine("3 - Editar Profissional");
            Console.WriteLine("4 - Excluir Profissional");
            Console.WriteLine("0 - Voltar ao Menu Principal");
            Console.Write("Opção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await CadastrarProfissional(context);
                    break;
                case "2":
                    await ListarProfissionais(context);
                    break;
                case "3":
                    await EditarProfissional(context);
                    break;
                case "4":
                    await ExcluirProfissional(context);
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
            }
        }
    }

    private static async Task CadastrarProfissional(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Cadastrar Profissional ===\n");

        var profissional = new Profissional();

        Console.Write("Nome: ");
        profissional.Nome = Console.ReadLine() ?? "";

        Console.Write("Telefone: ");
        profissional.Telefone = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        profissional.Email = Console.ReadLine() ?? "";

        DateTime dataNascimento;
        while (true)
        {
            Console.Write("Data de Nascimento (dd/mm/aaaa): ");
            var entradaData = Console.ReadLine();

            if (DateTime.TryParseExact(entradaData, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNascimento))
            {
                profissional.DataNascimento = dataNascimento;
                break;
            }
            else
            {
                Console.WriteLine("Data inválida! Por favor, insira no formato dd/mm/aaaa.");
            }
        }

        Console.Write("Especialidade: ");
        profissional.Especialidade = Console.ReadLine() ?? "";

        Console.WriteLine("\nCategorias disponíveis:");
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

        Console.WriteLine("\nProfissional cadastrado com sucesso!");
    }

    private static async Task ListarProfissionais(SalaoContext context, CategoriaServico? categoria = null)
    {
        Console.Clear();
        Console.WriteLine("=== Lista de Profissionais ===\n");

        var query = context.Profissionais.AsQueryable();

        if (categoria.HasValue)
        {
            query = query.Where(p => p.Categoria == categoria.Value);
            Console.WriteLine($"(Filtrando por categoria: {categoria})\n");
        }

        var profissionais = await query.ToListAsync();

        if (!profissionais.Any())
        {
            Console.WriteLine("Nenhum profissional encontrado.");
            return;
        }

        foreach (var profissional in profissionais)
        {
            Console.WriteLine(profissional);
            Console.WriteLine("----------------------------------");
        }
    }

    private static async Task EditarProfissional(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Editar Profissional ===\n");

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
            Console.Clear();
            Console.WriteLine($"Profissional atual:\n{profissional}\n");
            Console.WriteLine("Qual campo deseja editar?");
            Console.WriteLine("1 - Nome");
            Console.WriteLine("2 - Telefone");
            Console.WriteLine("3 - Email");
            Console.WriteLine("4 - Especialidade");
            Console.WriteLine("5 - Categoria");
            Console.WriteLine("6 - Hora Início de Trabalho");
            Console.WriteLine("7 - Hora Fim de Trabalho");
            Console.WriteLine("0 - Finalizar edição");

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

            if (opcao != "0")
            {
                Console.WriteLine("\nCampo atualizado! Pressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("\nProfissional editado com sucesso!");
    }

    private static async Task ExcluirProfissional(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Excluir Profissional ===\n");

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
            Console.WriteLine("\nProfissional excluído com sucesso!");
        }
        else
        {
            Console.WriteLine("\nExclusão cancelada.");
        }
    }
}
