using System;
using GestaoSalaoDeBeleza.Models;

namespace GestaoSalaoDeBeleza.Menu;

public class MenuCadastroGenerico
{
    public static void Executar()
    {
        string? opcao;
        do
        {
            Console.WriteLine("==== MENU PRINCIPAL ====");
            Console.WriteLine("1 - Gerenciar Clientes");
            Console.WriteLine("2 - Gerenciar Profissionais");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha: ");
            opcao = Console.ReadLine();

            Console.WriteLine();

            switch (opcao)
            {
                case "1": MenuCadastroPessoas<Cliente>.Executar("Cliente", cliente => { }); break;
                case "2":
                    MenuCadastroPessoas<Profissional>.Executar("Profissional", profissional =>
                    {
                        Console.Write("Deseja adicionar especialidade? (s/n): ");
                        if (Console.ReadLine()?.Trim().ToLower() == "s")
                        {
                            bool adicionar = true;
                            while (adicionar)
                            {
                                Console.WriteLine("Especialidades disponíveis:");
                                foreach (var esp in Enum.GetValues<Especialidade>())
                                    Console.WriteLine($"{(int)esp} - {esp}");

                                Console.Write("Escolha o número da especialidade: ");
                                if (int.TryParse(Console.ReadLine(), out int escolha) &&
                                    Enum.IsDefined(typeof(Especialidade), escolha))
                                {
                                    profissional.AdicionarEspecialidade((Especialidade)escolha);
                                }
                                else
                                {
                                    Console.WriteLine("Especialidade inválida.");
                                }

                                Console.Write("Deseja adicionar outra especialidade? (s/n): ");
                                adicionar = Console.ReadLine()?.Trim().ToLower() == "s";
                            }
                        }

                        Console.WriteLine("Deseja adicionar horário(s) de trabalho? (s/n)");
                        if (Console.ReadLine()?.Trim().ToLower() == "s")
                        {
                             Console.WriteLine("Horários de trabalho já cadastrados:");
                        if (profissional.HorarioTrabalho.Count == 0)
                        {
                            Console.WriteLine("Nenhum horário cadastrado.");
                        }
                        else
                        {
                            foreach (var h in profissional.HorarioTrabalho)
                            {
                                Console.WriteLine($"- {h.HoraInicio:hh\\:mm} às {h.HoraFim:hh\\:mm}");
                            }
                        }

                            bool adicionarHorario = true;
                            while (adicionarHorario)
                            {
                                TimeSpan inicio;
                                TimeSpan fim;

                                while (true)
                                {
                                    Console.Write("Hora início (HH:mm): ");
                                    string? entradaInicio = Console.ReadLine();
                                    if (TimeSpan.TryParseExact(entradaInicio, @"hh\:mm", null, out inicio))
                                        break;
                                    Console.WriteLine("Horário inválido. Use o formato HH:mm, ex: 08:30.");
                                }

                                while (true)
                                {
                                    Console.Write("Hora fim (HH:mm): ");
                                    string? entradaFim = Console.ReadLine();
                                    if (TimeSpan.TryParseExact(entradaFim, @"hh\:mm", null, out fim))
                                        break;
                                    Console.WriteLine("Horário inválido. Use o formato HH:mm");
                                }

                                if (inicio < fim)
                                {
                                    if (!profissional.HorariosConflitam(inicio, fim))
                                    {
                                        profissional.TentarAdicionarHorario(new HorarioTrabalho(inicio, fim));
                                    }
                                    else
                                    {
                                        Console.WriteLine("Este horário conflita com um horário já cadastrado. Tente outro.");
                                        continue; 
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Hora início deve ser menor que hora fim. Tente novamente.\n");
                                    continue;
                                }

                                Console.WriteLine("Horários de trabalho já cadastrados:");
                        if (profissional.HorarioTrabalho.Count == 0)
                        {
                            Console.WriteLine("Nenhum horário cadastrado.");
                        }
                        else
                        {
                            foreach (var h in profissional.HorarioTrabalho)
                            {
                                Console.WriteLine($"- {h.HoraInicio:hh\\:mm} às {h.HoraFim:hh\\:mm}");
                            }
                        }

                                Console.Write("Deseja adicionar outro horário de trabalho? (s/n): ");
                                adicionarHorario = Console.ReadLine()?.Trim().ToLower() == "s";
                            }
                        }
                    });
                    break;

                case "0":
                    Console.WriteLine("Encerrando...");
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opcao != "0");
    }


}
