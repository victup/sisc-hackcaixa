namespace SISC.Repositories.Simulacao
{
    public interface ISimulacaoRepository
    {
        Task AddAsync(SISC.Models.Simulacao.Simulacao simulacao);
        Task<IEnumerable<SISC.Models.Simulacao.Simulacao>> GetAllAsync();
        Task<IEnumerable<SISC.Models.Simulacao.Simulacao>> GetByDateAsync(DateTime date);
    }
}
