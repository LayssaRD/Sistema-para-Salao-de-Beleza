
using GestaoSalaoDeBeleza.Data;
using GestaoSalaoDeBeleza.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza.Menus;

public class MenuServico
{
    public static async Task ExibirMenu(SalaoContext context)
    {
        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("=== GESTÃO DE SERVIÇOS ===\n");
            Console.WriteLine("1 - Cadastrar Serviço");
            Console.WriteLine("2 - Listar Serviços");
            Console.WriteLine("3 - Editar Serviço");
            Console.WriteLine("4 - Excluir Serviço");
            Console.WriteLine("0 - Voltar ao Menu Principal");
            Console.Write("Opção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await CadastrarServico(context);
                    break;
                case "2":
                    await ListarServicos(context);
                    break;
                case "3":
                    await EditarServico(context);
                    break;
                case "4":
                    await ExcluirServico(context);
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

    private static async Task CadastrarServico(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Cadastrar Novo Serviço ===\n");

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

        Console.WriteLine("\nCategorias disponíveis:");
        foreach (var cat in Enum.GetValues(typeof(CategoriaServico)))
            Console.WriteLine($"{(int)cat} - {cat}");

        Console.Write("Selecione o número da categoria: ");
        if (!int.TryParse(Console.ReadLine(), out int categoriaSelecionada) || 
            !Enum.IsDefined(typeof(CategoriaServico), categoriaSelecionada))
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

        Console.WriteLine("\nServiço cadastrado com sucesso!");
    }

    private static async Task ListarServicos(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Lista de Serviços ===\n");

        var servicos = await context.Servicos.ToListAsync();

        if (!servicos.Any())
        {
            Console.WriteLine("Nenhum serviço cadastrado.");
            return;
        }

        foreach (var servico in servicos)
        {
            Console.WriteLine(servico);
            Console.WriteLine("----------------------------------");
        }
    }

    private static async Task EditarServico(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Editar Serviço ===\n");

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
            Console.Clear();
            Console.WriteLine($"Serviço atual:\n{servico}\n");
            Console.WriteLine("Qual campo deseja editar?");
            Console.WriteLine("1 - Nome");
            Console.WriteLine("2 - Preço");
            Console.WriteLine("3 - Duração (em minutos)");
            Console.WriteLine("4 - Categoria");
            Console.WriteLine("0 - Finalizar edição");

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

            if (opcao != "0")
            {
                Console.WriteLine("\nCampo atualizado! Pressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("\nServiço editado com sucesso!");
    }

    private static async Task ExcluirServico(SalaoContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Excluir Serviço ===\n");

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
            Console.WriteLine("\nServiço excluído com sucesso!");
        }
        else
        {
            Console.WriteLine("\nExclusão cancelada.");
        }
    }
}
