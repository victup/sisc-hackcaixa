using Microsoft.EntityFrameworkCore;
using SISC.Data;

namespace SISC.Repositories.Simulacao
{
    public class SimulacaoRepository : ISimulacaoRepository
    {
        private readonly SimulacoesDbContext _context;

        public SimulacaoRepository(SimulacoesDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Models.Simulacao.Simulacao simulacao)
        {
            _context.Simulacoes.Add(simulacao);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Models.Simulacao.Simulacao>> GetAllAsync()
        {
            return await _context.Simulacoes
                .Include(s => s.Resultados)
                    .ThenInclude(r => r.Parcelas)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Simulacao.Simulacao>> GetByDateAsync(DateTime date)
        {
            return await _context.Simulacoes
                .Where(s => s.DataCriacao.Date == date.Date)
                .Include(s => s.Resultados)
                    .ThenInclude(r => r.Parcelas)
                .ToListAsync();
        }
    }
}
