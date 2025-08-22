using SISC.DTOs.Simulacao;

namespace SISC.DTOs.Responses.Simulacao
{
    public class SimulacaoResponse
    {
        public long IdSimulacao { get; set; }
        public int CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; } = string.Empty;
        public decimal TaxaJuros { get; set; }
        public decimal ValorDesejado { get; set; }
        public int Prazo { get; set; }
        public DateTime DataCriacao { get; set; }
        public IEnumerable<ResultadoSimulacaoResponse> ResultadoSimulacao { get; set; } = new List<ResultadoSimulacaoResponse>();
    }
}
