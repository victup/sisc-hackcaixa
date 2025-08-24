namespace SISC.DTOs.Simulacao
{
    public class ResultadoSimulacaoResumidoResponse
    {
        public long IdSimulacao { get; set; }
        public decimal valorDesejado { get; set; }
        public int Prazo { get; set; }
        public decimal valorTotalParcelas { get; set; }

    }
}
