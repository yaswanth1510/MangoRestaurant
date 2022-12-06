using Mango.Services.ShoppingCartAPI.Domain.Specifications;

namespace Mango.Services.ShoppingCartAPI.Domain.Interface
{
    public interface ICartRepository<T> where T : class
    {
        Task<T> GetById(string id);
        Task<T> GetById(int id);
        Task<(T, bool)> Create(T entity);
        Task<(T, bool)> Update(T entity);
        Task<bool> Remove(T entity);
        Task<bool> Clear(List<T> entity);
        IQueryable<T> Query();
        Task<T> FirstOrDefaultAsync(ISpecification<T> spec);
    }
}
