namespace SISC.DTOs.Telemetria
{
    public class TelemetriaResponse
    {
        public DateTime DataReferencia { get; set; }
        public List<EndpointTelemetriaResponse> ListaEndpoints { get; set; } = new();
    }
}
