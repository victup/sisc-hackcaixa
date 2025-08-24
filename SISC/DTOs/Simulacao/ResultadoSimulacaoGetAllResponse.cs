namespace SISC.DTOs.Simulacao
{
    public class ResultadoSimulacaoGetAllResponse
    {
        public long IdSimulacao { get; set; }
        public decimal valorDesejado { get; set; }
        public int Prazo { get; set; }
        public decimal valorTotalParcelas { get; set; }

    }
}
