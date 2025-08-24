namespace SISC.DTOs.Simulacao
{
    public class SimulacaoGetAllResponse
    {
        public long pagina { get; set; }
        public long qtdRegistros { get; set; }
        public long qtdRegistrosPagina { get; set; }
        public IEnumerable<ResultadoSimulacaoResumidoResponse> registros { get; set; } = new List<ResultadoSimulacaoResumidoResponse>();
    }
}
