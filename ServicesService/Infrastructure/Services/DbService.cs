using Microsoft.EntityFrameworkCore;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Exceptions;
using ServicesService.Domain.Interfaces;
using ServicesService.Infrastructure.Data;

namespace ServicesService.Infrastructure.Services
{
    public class DbService : IServiceDBService, ISpecializationDBService
    {
        private readonly ServicesDbContext _dbContext;

        public DbService(ServicesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Service> Add(Service service)
        {
            service.Id = Guid.NewGuid();

            _dbContext.Services.Add(service);

            await _dbContext.SaveChangesAsync();

            return service;
        }

        public async Task<Service> Delete(Guid id)
        {
            var service = await _dbContext.Services.FirstOrDefaultAsync(o => o.Id == id);

            if (service == null)
            {
                throw new ServicesException("Service not found", 404);
            }

            _dbContext.Services.Remove(service);

            await _dbContext.SaveChangesAsync();

            return service;
        }

        public async Task<Service> Get(Guid id)
        {
            var service = await _dbContext.Services.FirstOrDefaultAsync(d => d.Id == id);

            if (service == null)
            {
                throw new ServicesException("Service not found", 404);
            }

            return service;
        }

        public async Task<IEnumerable<Service>> Get(int page, int pageSize, IEnumerable<Guid>? categories)
        {
            IQueryable<Service> query = _dbContext.Services;

            if (categories != null && categories.Any())
            {
                query = query.Where(s => categories.Contains(s.CategoryId));
            }

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            IEnumerable<Service> services = await query.AsNoTracking().ToListAsync();

            return services;
        }

        public async Task<Service> Update(Service service)
        {
            var toEdit = await _dbContext.Services.FirstOrDefaultAsync(o => o.Id == service.Id);

            if (toEdit == null)
            {
                throw new ServicesException("Services not found", 404);
            }

            _dbContext.Entry(toEdit).CurrentValues.SetValues(service);

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }


        public async Task<Specialization> Add(Specialization service)
        {
            service.Id = Guid.NewGuid();

            _dbContext.Specializations.Add(service);

            await _dbContext.SaveChangesAsync();

            return service;
        }

        public async Task<Specialization> Update(Specialization service)
        {
            var toEdit = await _dbContext.Specializations.FirstOrDefaultAsync(o => o.Id == service.Id);

            if (toEdit == null)
            {
                throw new ServicesException("Specialization not found", 404);
            }

            _dbContext.Entry(toEdit).CurrentValues.SetValues(service);

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }

        async Task<Specialization> ISpecializationDBService.Delete(Guid id)
        {
            var service = await _dbContext.Specializations.FirstOrDefaultAsync(o => o.Id == id);

            if (service == null)
            {
                throw new ServicesException("Specialization not found", 404);
            }

            _dbContext.Specializations.Remove(service);

            await _dbContext.SaveChangesAsync();

            return service;
        }

        async Task<Specialization> ISpecializationDBService.Get(Guid id)
        {
            var service = await _dbContext.Specializations.FirstOrDefaultAsync(d => d.Id == id);

            if (service == null)
            {
                throw new ServicesException("Specialization not found", 404);
            }

            return service;
        }

        async Task<IEnumerable<Specialization>> ISpecializationDBService.Get(int page, int pageSize)
        {
            IQueryable<Specialization> query = _dbContext.Specializations;

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            IEnumerable<Specialization> services = await query.ToListAsync();

            return services;
        }
    }
}
