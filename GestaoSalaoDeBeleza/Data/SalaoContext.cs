using GestaoSalaoDeBeleza.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoSalaoDeBeleza.Data;

public class SalaoContext: DbContext
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Profissional> Profissionais { get; set; }
    public DbSet<ServicoOfertado> Servicos { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=salao.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>().ToTable("Pessoas");
        modelBuilder.Entity<Cliente>().ToTable("Clientes");
        modelBuilder.Entity<Profissional>().ToTable("Profissionais");

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefone).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.Property(e => e.Endereco).HasMaxLength(200);
        });
        
        modelBuilder.Entity<Profissional>(entity =>
        {
            entity.Property(e => e.Especialidade).HasMaxLength(50);
            entity.Property(e => e.Categoria).HasConversion<string>();
        });
        
        modelBuilder.Entity<ServicoOfertado>(entity =>
        {
            entity.HasKey(e => e.ServicoId);
            entity.Property(e => e.Nome).IsRequired();
            entity.Property(e => e.Preco).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Categoria).HasConversion<string>();
        });
        
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasKey(e => e.AgendamentoId);
            
            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.HistoricoAgendamentos)
                .HasForeignKey(e => e.ClienteId);
                
            entity.HasOne(e => e.Profissional)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(e => e.ProfissionalId);
                
            entity.HasOne(e => e.Servico)
                .WithMany(s => s.Agendamentos)
                .HasForeignKey(e => e.ServicoId);
        });
        
        //SeedData(modelBuilder);
    }
    
    // private static void SeedData(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<ServicoOfertado>().HasData(
    //         new ServicoOfertado { ServicoId = 1, Nome = "Corte de Cabelo", Preco = 25.00m, DuracaoMinutos = 30, Descricao = "Corte feminino e masculino" },
    //         new ServicoOfertado { ServicoId = 2, Nome = "Manicure", Preco = 15.00m, DuracaoMinutos = 45, Descricao = "Cuidados com as unhas das mãos" },
    //         new ServicoOfertado { ServicoId = 3, Nome = "Pedicure", Preco = 20.00m, DuracaoMinutos = 60, Descricao = "Cuidados com as unhas dos pés" },
    //         new ServicoOfertado { ServicoId = 4, Nome = "Escova", Preco = 20.00m, DuracaoMinutos = 40, Descricao = "Escova modeladora" },
    //         new ServicoOfertado { ServicoId = 5, Nome = "Design de Sobrancelha", Preco = 12.00m, DuracaoMinutos = 20, Descricao = "Modelagem de sobrancelhas" }
    //     );
        
    //     modelBuilder.Entity<Profissional>().HasData(
    //         new Profissional 
    //         { 
    //             Id = 01, 
    //             Nome = "Maria Silva", 
    //             Telefone = "(11) 99999-1111", 
    //             Email = "maria@salao.com",
    //             DataNascimento = new DateTime(1985, 5, 15),
    //             Especialidade = "Cabelo",
    //             HoraInicioTrabalho = new TimeSpan(8, 0, 0),
    //             HoraFimTrabalho = new TimeSpan(18, 0, 0)
    //         },
    //         new Profissional 
    //         { 
    //             Id = 2, 
    //             Nome = "Ana Costa", 
    //             Telefone = "(11) 99999-2222", 
    //             Email = "ana@salao.com",
    //             DataNascimento = new DateTime(1990, 8, 22),
    //             Especialidade = "Unhas",
    //             HoraInicioTrabalho = new TimeSpan(9, 0, 0),
    //             HoraFimTrabalho = new TimeSpan(17, 0, 0)
    //         }
    //     );
    // }
}
