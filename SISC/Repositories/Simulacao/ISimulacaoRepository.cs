namespace SISC.Repositories.Simulacao
{
    public interface ISimulacaoRepository
    {
        Task AddAsync(Models.Simulacao.Simulacao simulacao);
        Task<IEnumerable<Models.Simulacao.Simulacao>> GetAllAsync();
        Task<IEnumerable<Models.Simulacao.Simulacao>> GetByDateAsync(DateTime date);
    }
}
