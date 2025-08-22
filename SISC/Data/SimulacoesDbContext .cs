using Microsoft.EntityFrameworkCore;
using SISC.Models.Simulacao;

namespace SISC.Data
{
    public class SimulacoesDbContext : DbContext
    {
        public SimulacoesDbContext(DbContextOptions<SimulacoesDbContext> options) : base(options) { }

        public DbSet<Simulacao> Simulacoes { get; set; }
        public DbSet<ResultadoSimulacao> Resultados { get; set; }
        public DbSet<Parcela> Parcelas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Simulacao ---
            modelBuilder.Entity<Simulacao>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Simulacao>()
                .Property(s => s.ValorDesejado).HasPrecision(18, 2);

            modelBuilder.Entity<Simulacao>()
                .Property(s => s.TaxaJuros).HasPrecision(9, 6);

            modelBuilder.Entity<Simulacao>()
                .HasMany(s => s.Resultados)
                .WithOne(r => r.Simulacao)
                .HasForeignKey(r => r.SimulacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- ResultadoSimulacao ---
            modelBuilder.Entity<ResultadoSimulacao>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<ResultadoSimulacao>()
                .HasMany(r => r.Parcelas)
                .WithOne(p => p.Resultado)
                .HasForeignKey(p => p.ResultadoSimulacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Parcela ---
            modelBuilder.Entity<Parcela>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Parcela>()
                .Property(p => p.ValorAmortizacao).HasPrecision(18, 2);

            modelBuilder.Entity<Parcela>()
                .Property(p => p.ValorJuros).HasPrecision(18, 2);

            modelBuilder.Entity<Parcela>()
                .Property(p => p.ValorPrestacao).HasPrecision(18, 2);
        }
    }
}
