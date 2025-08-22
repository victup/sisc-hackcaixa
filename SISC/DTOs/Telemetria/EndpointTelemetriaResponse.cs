namespace SISC.DTOs.Telemetria
{
    public class EndpointTelemetriaResponse
    {
        public string NomeApi { get; set; } = string.Empty;
        public int QtdRequisicoes { get; set; }
        public long TempoMedio { get; set; }
        public long TempoMinimo { get; set; }
        public long TempoMaximo { get; set; }
        public double PercentualSucesso { get; set; }
    }
}
