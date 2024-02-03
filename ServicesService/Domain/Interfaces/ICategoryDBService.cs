using ServicesService.Domain.Entities;

namespace ServicesService.Domain.Interfaces
{
    public interface ICategoryDBService
    {
        Task<Category> Get(Guid id);
        Task<IEnumerable<Category>> Get(int page, int pageSize);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task Delete(Guid id);
    }
}
