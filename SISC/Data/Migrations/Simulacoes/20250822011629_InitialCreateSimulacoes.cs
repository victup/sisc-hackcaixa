using System;
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodigoProduto = table.Column<int>(type: "INTEGER", nullable: false),
                    DescricaoProduto = table.Column<string>(type: "TEXT", nullable: false),
                    TaxaJuros = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: false),
                    ValorDesejado = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Prazo = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resultados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    SimulacaoId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resultados_Simulacoes_SimulacaoId",
                        column: x => x.SimulacaoId,
                        principalTable: "Simulacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parcelas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorAmortizacao = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ValorJuros = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ValorPrestacao = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ResultadoSimulacaoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcelas_Resultados_ResultadoSimulacaoId",
                        column: x => x.ResultadoSimulacaoId,
                        principalTable: "Resultados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_ResultadoSimulacaoId",
                table: "Parcelas",
                column: "ResultadoSimulacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_SimulacaoId",
                table: "Resultados",
                column: "SimulacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcelas");

            migrationBuilder.DropTable(
                name: "Resultados");

            migrationBuilder.DropTable(
                name: "Simulacoes");
        }
    }
}
