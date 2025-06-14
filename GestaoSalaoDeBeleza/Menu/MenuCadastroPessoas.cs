using System;
using GestaoSalaoDeBeleza.Models;
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
            Console.WriteLine($"5 - Atualizar {nomeEntidade}");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha: ");
            opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": Adicionar(preencherCamposExtras); break;
                case "2": Listar(); break;
                case "3": BuscarPorId(); break;
                case "4": Remover(); break;
                case "5": Atualizar(preencherCamposExtras); break;
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
                while (true)
                {
                    Console.Write($"{prop.Name}: ");
                    string? valor = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        prop.SetValue(entidade, valor);
                        break;
                    }

                    Console.WriteLine("O valor não pode ser vazio. Tente novamente.");
                }
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                while (true)
                {
                    Console.Write($"{prop.Name} (dd/MM/yyyy): ");
                    string? input = Console.ReadLine();

                    if (DateTime.TryParseExact(
                            input,
                            "dd/MM/yyyy",
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime dt))
                    {
                        if (dt < DateTime.Today)
                        {
                            prop.SetValue(entidade, dt);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("A data deve ser anterior a hoje. Tente novamente.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Data inválida. Use o formato dd/MM/yyyy.");
                    }
                }
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

        int contador = 1;
        foreach (var item in lista)
        {
            Console.WriteLine($"\nID Representativo: {contador}");
            Console.WriteLine(item);
            Console.WriteLine("-----------------------------------------");
            contador++;
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

    private static void Atualizar(Action<T>? preencherExtras)
{
    Console.Write("Digite o ID da entidade que deseja atualizar: ");
    if (!Guid.TryParse(Console.ReadLine(), out Guid id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    var entidade = _repo.ObterPorId(id);
    if (entidade == null)
    {
        Console.WriteLine("Entidade não encontrada.");
        return;
    }

    Console.WriteLine("Atualize os campos (pressione ENTER para manter o valor atual):");

    foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
    {
        if (!prop.CanWrite || prop.Name == "Id") continue;

        object? valorAtual = prop.GetValue(entidade);

        if (prop.PropertyType == typeof(string))
        {
            string? atual = valorAtual as string;
            while (true)
            {
                Console.Write($"{prop.Name} ({atual ?? "vazio"}): ");
                string? novoValor = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(novoValor))
                    break;

                prop.SetValue(entidade, novoValor);
                break;
            }
        }
        else if (prop.PropertyType == typeof(DateTime))
        {
            DateTime? atual = valorAtual as DateTime?;
            while (true)
            {
                Console.Write($"{prop.Name} ({(atual.HasValue ? atual.Value.ToString("dd/MM/yyyy") : "vazio")}): ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    break;

                if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    if (dt < DateTime.Today)
                    {
                        prop.SetValue(entidade, dt);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("A data deve ser anterior a hoje.");
                    }
                }
                else
                {
                    Console.WriteLine("Formato inválido. Use dd/MM/yyyy.");
                }
            }
        }
        else if (prop.PropertyType == typeof(int))
        {
            int? atual = valorAtual as int?;
            while (true)
            {
                Console.Write($"{prop.Name} ({(atual.HasValue ? atual.ToString() : "vazio")}): ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    break;

                if (int.TryParse(input, out int inteiro))
                {
                    prop.SetValue(entidade, inteiro);
                    break;
                }

                Console.WriteLine("Número inválido. Digite um número inteiro.");
            }
        }
    }

    // 🔁 Este bloco deve ficar fora do foreach das propriedades
    if (entidade is Profissional profissional)
    {
        // ---- Remover especialidade ----
        Console.Write("Deseja remover uma especialidade existente? (s/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "s")
        {
            if (profissional.Especialidades.Count == 0)
            {
                Console.WriteLine("Este profissional não possui especialidades cadastradas.");
            }
            else
            {
                foreach (var esp in profissional.Especialidades)
                    Console.WriteLine($"{(int)esp} - {esp}");

                Console.Write("Digite o número da especialidade que deseja remover: ");
                if (int.TryParse(Console.ReadLine(), out int cod) &&
                    Enum.IsDefined(typeof(Especialidade), cod))
                {
                    profissional.RemoverEspecialidade((Especialidade)cod);
                }
                else
                {
                    Console.WriteLine("Especialidade inválida.");
                }
            }
        }

        // ---- Remover horário de trabalho ----
        Console.Write("Deseja remover um horário de trabalho? (s/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "s")
        {
            if (profissional.HorarioTrabalho.Count == 0)
            {
                Console.WriteLine("Este profissional não possui horários cadastrados.");
            }
            else
            {
                for (int i = 0; i < profissional.HorarioTrabalho.Count; i++)
                {
                    var h = profissional.HorarioTrabalho[i];
                    Console.WriteLine($"{i} - {h.HoraInicio:hh\\:mm} às {h.HoraFim:hh\\:mm}");
                }

                Console.Write("Digite o número do horário que deseja remover: ");
                if (int.TryParse(Console.ReadLine(), out int indice) &&
                    indice >= 0 && indice < profissional.HorarioTrabalho.Count)
                {
                    profissional.RemoverHorarioTrabalho(profissional.HorarioTrabalho[indice]);
                }
                else
                {
                    Console.WriteLine("Índice de horário inválido.");
                }
            }
        }
    }

    preencherExtras?.Invoke(entidade);

    _repo.Atualizar(entidade);
    Console.WriteLine($"{typeof(T).Name} atualizado com sucesso.");
}

}
