using Microsoft.EntityFrameworkCore;
using SISC.Models.Produto;

namespace SISC.Data
{
    public class ProdutoDbContext : DbContext
    {
        public ProdutoDbContext(DbContextOptions<ProdutoDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.ToTable("PRODUTO");

                entity.HasKey(p => p.CoProduto);

                entity.Property(p => p.CoProduto)
                      .HasColumnName("CO_PRODUTO");

                entity.Property(p => p.NoProduto)
                      .HasColumnName("NO_PRODUTO")
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(p => p.PcTaxaJuros)
                      .HasColumnName("PC_TAXA_JUROS")
                      .HasPrecision(16, 9);

                entity.Property(p => p.NuMinimoMeses)
                      .HasColumnName("NU_MINIMO_MESES");

                entity.Property(p => p.NuMaximoMeses)
                      .HasColumnName("NU_MAXIMO_MESES");

                entity.Property(p => p.VrMinimo)
                      .HasColumnName("VR_MINIMO")
                      .HasPrecision(18, 2);

                entity.Property(p => p.VrMaximo)
                      .HasColumnName("VR_MAXIMO")
                      .HasPrecision(18, 2);
            });
        }
    }
}
