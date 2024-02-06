namespace DocumentsService.Domain.Interfaces
{
    public interface IFilesRepository
    {
        Task<Entities.File> Get(Guid id);
        Task<IEnumerable<Entities.File>> Get(int page, int pageSize, string? Name, Guid? Owner);
        Task<Guid> Add(Entities.File document);
        Task<Guid> Update(Entities.File document);
        Task<Guid> Rename(Guid id, string newName);
        Task Delete(Guid id);
    }
}
