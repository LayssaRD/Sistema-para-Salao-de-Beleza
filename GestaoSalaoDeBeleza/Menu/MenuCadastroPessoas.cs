using System;
using System.Reflection;
using GenericRepository.Entidade;
using GenericRepository.Implementacoes;

namespace GestaoSalaoDeBeleza.Menu;

public static class MenuCadastroPessoas<T> where T : class, IEntidade, new()
{
    private static readonly GenericJsonRepository<T> _repo = new();

    public static void Executar(string nomeEntidade, Action<T>? preencherCamposExtras = null)
    {
        string? opcao;
        do
        {
            Console.WriteLine($"=== MENU {nomeEntidade.ToUpper()} ===");
            Console.WriteLine($"1 - Adicionar {nomeEntidade}");
            Console.WriteLine($"2 - Listar {nomeEntidade}s");
            Console.WriteLine($"3 - Buscar {nomeEntidade} por ID");
            Console.WriteLine($"4 - Remover {nomeEntidade}");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha: ");
            opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": Adicionar(preencherCamposExtras); break;
                case "2": Listar(); break;
                case "3": BuscarPorId(); break;
                case "4": Remover(); break;
                case "0": Console.WriteLine("Voltando..."); break;
                default: Console.WriteLine("Opção inválida."); break;
            }

            if (opcao != "0")
            {
                Console.WriteLine("\nPressione ENTER para continuar...");
                Console.ReadLine();
            }
            
        } while (opcao != "0");
    }

    private static void Adicionar(Action<T>? preencherExtras)
    {
        var entidade = new T();

        Console.WriteLine("Preencha os campos:");

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanWrite || prop.Name == "Id") continue;

            if (prop.PropertyType == typeof(string))
            {
                Console.Write($"{prop.Name}: ");
                string? valor = Console.ReadLine();
                prop.SetValue(entidade, valor);
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                Console.Write($"{prop.Name} (dd/MM/yyyy): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime dt))
                    prop.SetValue(entidade, dt);
            }
            else if (prop.PropertyType == typeof(int))
            {
                Console.Write($"{prop.Name}: ");
                if (int.TryParse(Console.ReadLine(), out int inteiro))
                    prop.SetValue(entidade, inteiro);
            }

        }

        preencherExtras?.Invoke(entidade);

        _repo.Adicionar(entidade);
        Console.WriteLine($"{typeof(T).Name} adicionado com sucesso.");
    }

    private static void Listar()
    {
        var lista = _repo.ObterTodos();

        if (!lista.Any())
        {
            Console.WriteLine("Nenhum registro encontrado.");
            return;
        }

        foreach (var item in lista)
        {
            Console.WriteLine(item);
            Console.WriteLine("-----------------------------------------");
        }
    }

    private static void BuscarPorId()
    {
        Console.Write("Digite o ID: ");
        if (Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            var item = _repo.ObterPorId(id);
            Console.WriteLine(item != null ? item.ToString() : "Registro não encontrado.");
        }
        else
        {
            Console.WriteLine("ID inválido.");
        }
    }

    private static void Remover()
    {
        Console.Write("Digite o ID para remover: ");
        if (Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            bool removido = _repo.Remover(id);
            Console.WriteLine(removido ? "Removido com sucesso." : "Registro não encontrado.");
        }
        else
        {
            Console.WriteLine("ID inválido.");
        }
    }
}
