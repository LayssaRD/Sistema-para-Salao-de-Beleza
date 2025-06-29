using GestaoSalaoDeBeleza.Data;
using GestaoSalaoDeBeleza.Menus;

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
                MostrarMenuPrincipal();
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        await MenuCliente.ExibirMenu(context);
                        break;
                    case "2":
                        await MenuProfissional.ExibirMenu(context);
                        break;
                    case "3":
                        await MenuServico.ExibirMenu(context);
                        break;
                    case "4":
                        await MenuAgendamento.ExibirMenu(context);
                        break;
                    case "0":
                        continuar = false;
                        Console.WriteLine("Encerrando o sistema...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu principal...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("=== MENU PRINCIPAL ===\n");
            Console.WriteLine("Escolha o módulo que deseja acessar:");
            Console.WriteLine();
            Console.WriteLine("1 - Gestão de Clientes");
            Console.WriteLine("2 - Gestão de Profissionais");
            Console.WriteLine("3 - Gestão de Serviços");
            Console.WriteLine("4 - Gestão de Agendamentos");
            Console.WriteLine();
            Console.WriteLine("0 - Sair do Sistema");
            Console.WriteLine();
            Console.Write("Opção: ");
        }
    }
}
