using ServicesService.Domain.Entities;

namespace ServicesService.Domain.Interfaces
{
    public interface ISpecializationDBService
    {
        Task<Specialization> Get(Guid id);
        Task<IEnumerable<Specialization>> Get(int page, int pageSize);
        Task<Specialization> Add(Specialization service);
        Task<Specialization> Update(Specialization service);
        Task<Specialization> Delete(Guid id);
    }
}
