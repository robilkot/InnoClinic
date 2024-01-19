using ServicesService.Domain.Entities;

namespace ServicesService.Domain.Interfaces
{
    public interface IServiceDBService
    {
        Task<Service> Get(Guid id);
        Task<IEnumerable<Service>> Get(int page, int pageSize, IEnumerable<Guid>? categories);
        Task<Service> Add(Service service);
        Task<Service> Update(Service service);
        Task<Service> Delete(Guid id);
    }
}
