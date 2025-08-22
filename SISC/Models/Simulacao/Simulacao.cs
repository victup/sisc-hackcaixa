using System.ComponentModel;

namespace SISC.Models.Simulacao
{
    public class Simulacao
    {
        public long Id { get; set; }
        public int CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; } = string.Empty;
        public decimal TaxaJuros { get; set; }
        public decimal ValorDesejado { get; set; }
        public int Prazo { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public ICollection<ResultadoSimulacao> Resultados { get; set; } = new List<ResultadoSimulacao>();
    }
}
