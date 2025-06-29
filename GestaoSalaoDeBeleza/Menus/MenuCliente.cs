using GestaoSalaoDeBeleza.Data;
using GestaoSalaoDeBeleza.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza.Menus;

public class MenuCliente
{
    public static async Task ExibirMenu(SalaoContext context)
        {
            bool continuar = true;
            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("=== GESTÃO DE CLIENTES ===\n");
                Console.WriteLine("1 - Cadastrar Cliente");
                Console.WriteLine("2 - Listar Clientes");
                Console.WriteLine("3 - Editar Cliente");
                Console.WriteLine("4 - Excluir Cliente");
                Console.WriteLine("0 - Voltar ao Menu Principal");
                Console.Write("Opção: ");

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

        private static async Task CadastrarCliente(SalaoContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Cadastrar Cliente ===\n");

            var cliente = new Cliente();

            Console.Write("Nome: ");
            cliente.Nome = Console.ReadLine() ?? "";

            Console.Write("Telefone: ");
            cliente.Telefone = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            cliente.Email = Console.ReadLine() ?? "";

            DateTime dataNascimento;
            while (true)
            {
                Console.Write("Data de Nascimento (dd/mm/aaaa): ");
                var entradaData = Console.ReadLine();

                if (DateTime.TryParseExact(entradaData, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNascimento))
                {
                    cliente.DataNascimento = dataNascimento;
                    break;
                }
                else
                {
                    Console.WriteLine("Data inválida! Por favor, insira no formato dd/mm/aaaa.");
                }
            }


            Console.Write("Endereço: ");
            cliente.Endereco = Console.ReadLine() ?? "";

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            Console.WriteLine("\nCliente cadastrado com sucesso!");
        }

        private static async Task ListarClientes(SalaoContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Lista de Clientes ===\n");

            var clientes = await context.Clientes.ToListAsync();

            if (!clientes.Any())
            {
                Console.WriteLine("Nenhum cliente cadastrado.");
                return;
            }

            foreach (var cliente in clientes)
            {
                Console.WriteLine(cliente);
                Console.WriteLine("----------------------------------");
            }
        }

        private static async Task EditarCliente(SalaoContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Editar Cliente ===\n");

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
                Console.WriteLine("0 - Finalizar edição");

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
            Console.WriteLine("\nAlterações salvas com sucesso!");
        }

        private static async Task ExcluirCliente(SalaoContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Excluir Cliente ===\n");

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
                Console.WriteLine("\nCliente excluído com sucesso!");
            }
            else
            {
                Console.WriteLine("\nExclusão cancelada.");
            }
        }
}
