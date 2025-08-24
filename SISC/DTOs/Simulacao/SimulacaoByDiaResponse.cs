namespace SISC.DTOs.Simulacao
{
    public class SimulacaoByDiaResponse
    {
        public DateTime DataReferencia { get; set; }
        public List<ResultadoSimulacaoByDiaResponse> Simulacoes { get; set; } = new();
    }
}
