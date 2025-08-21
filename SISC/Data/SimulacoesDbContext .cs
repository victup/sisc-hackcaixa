using Microsoft.EntityFrameworkCore;
using SISC.Models.Simulacao;

namespace SISC.Data
{
    public class SimulacoesDbContext : DbContext
    {
        public SimulacoesDbContext(DbContextOptions<SimulacoesDbContext> options) : base(options) { }

        public DbSet<Simulacao> Simulacoes { get; set; }
        public DbSet<Parcela> Parcelas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Simulacao>()
                .HasKey(s => s.IdSimulacao);

            modelBuilder.Entity<Parcela>()
                .HasKey(p => p.IdParcela);

            modelBuilder.Entity<Parcela>()
                .HasOne(p => p.Simulacao)
                .WithMany(s => s.Parcelas)
                .HasForeignKey(p => p.IdSimulacao)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Simulacao>()
                .Property(s => s.ValorDesejado).HasPrecision(18, 2);
            modelBuilder.Entity<Simulacao>()
                .Property(s => s.TaxaJuros).HasPrecision(9, 6);

            modelBuilder.Entity<Parcela>()
                .Property(p => p.ValorAmortizacao).HasPrecision(18, 2);
            modelBuilder.Entity<Parcela>()
                .Property(p => p.ValorJuros).HasPrecision(18, 2);
            modelBuilder.Entity<Parcela>()
                .Property(p => p.ValorPrestacao).HasPrecision(18, 2);
        }
    }
}
