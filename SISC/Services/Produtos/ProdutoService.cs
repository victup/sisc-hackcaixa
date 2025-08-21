using SISC.Models.Produto;
using SISC.Repositories.Produtos;

namespace SISC.Services.Produtos
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repository;

        public ProdutoService(IProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Produto>> ListarProdutosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Produto?> BuscarProdutoAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
