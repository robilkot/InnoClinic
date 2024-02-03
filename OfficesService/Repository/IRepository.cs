namespace OfficesService.Repository
{
    public interface IRepository<T>
    {
        Task<T> Get(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(Guid id);
        void Init();
    }
}
