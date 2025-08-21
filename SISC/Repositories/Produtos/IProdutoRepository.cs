using Microsoft.EntityFrameworkCore;
using SISC.Data;
using SISC.Models.Produto;

namespace SISC.Repositories.Produtos
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto?> GetByIdAsync(int id);
    }
}
