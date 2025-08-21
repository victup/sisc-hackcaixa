using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISC.Data.Migrations.Simulacoes
{
    /// <inheritdoc />
    public partial class InitialCreateSimulacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    IdSimulacao = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodigoProduto = table.Column<int>(type: "INTEGER", nullable: false),
                    DescricaoProduto = table.Column<string>(type: "TEXT", nullable: false),
                    ValorDesejado = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Prazo = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxaJuros = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.IdSimulacao);
                });

            migrationBuilder.CreateTable(
                name: "Parcelas",
                columns: table => new
                {
                    IdParcela = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorAmortizacao = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ValorJuros = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ValorPrestacao = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IdSimulacao = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcelas", x => x.IdParcela);
                    table.ForeignKey(
                        name: "FK_Parcelas_Simulacoes_IdSimulacao",
                        column: x => x.IdSimulacao,
                        principalTable: "Simulacoes",
                        principalColumn: "IdSimulacao",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_IdSimulacao",
                table: "Parcelas",
                column: "IdSimulacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcelas");

            migrationBuilder.DropTable(
                name: "Simulacoes");
        }
    }
}
