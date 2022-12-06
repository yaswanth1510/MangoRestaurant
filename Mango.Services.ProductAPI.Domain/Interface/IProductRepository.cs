namespace Mango.Services.ProductAPI.Domain.Interface
{
    public interface IProductRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T> GetById(int id);
        Task<(T, bool)> Create(T entity);
        Task<(T, bool)> Update(T entity);
        Task<bool> Delete(T entity);
        IQueryable<T> Query();
    }
}
