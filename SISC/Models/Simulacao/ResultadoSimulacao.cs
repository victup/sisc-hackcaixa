namespace SISC.Models.Simulacao
{
    public class ResultadoSimulacao
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public long SimulacaoId { get; set; }
        public Simulacao Simulacao { get; set; } = null!;
        public ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
    }
}
