namespace MapsterBookDemo.Application.Interfaces;
public interface IRepository<T, in TKey> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    //Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetAsync(TKey id);
    Task<T> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(TKey id);
}
