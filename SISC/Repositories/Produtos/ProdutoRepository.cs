using Microsoft.EntityFrameworkCore;
using SISC.Data;
using SISC.Models.Produto;

namespace SISC.Repositories.Produtos
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ProdutosDbContext _context;

        public ProdutoRepository(ProdutosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto?> GetByIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }
    }
}
