using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Exceptions;
using ServicesService.Domain.Interfaces;

namespace ServicesService.Infrastructure.Services
{
    public class DbService : IServiceDBService, ISpecializationDBService, ICategoryDBService
    {
        private readonly IMongoDatabase _dbContext;

        public DbService(IMongoDatabase dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Service> Add(Service service)
        {
            service.Id = Guid.NewGuid();

            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            await servicesCollection.InsertOneAsync(service);

            return service;
        }

        public async Task<Service> Delete(Guid id)
        {
            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            var service = await servicesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (service == null)
            {
                throw new ServicesException("Service not found", 404);
            }

            await servicesCollection.DeleteOneAsync(s => s.Id == id);

            return service;
        }

        public async Task<Service> Get(Guid id)
        {
            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            var service = await servicesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (service == null)
            {
                throw new ServicesException("Service not found", 404);
            }

            return service;
        }

        public async Task<IEnumerable<Service>> Get(int page, int pageSize, IEnumerable<Guid>? categories)
        {
            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            var filterBuilder = Builders<Service>.Filter;
            FilterDefinition<Service> filter = filterBuilder.Empty;

            if (categories != null && categories.Any())
            {
                filter &= filterBuilder.In(s => s.CategoryId, categories);
            }

            FindOptions<Service> options = new FindOptions<Service>();
            options.Skip = (page - 1) * pageSize;
            options.Limit = pageSize;

            using (var cursor = await servicesCollection.FindAsync(filter, options))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<Service> Update(Service service)
        {
            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            var toEdit = await servicesCollection.Find(s => s.Id == service.Id).FirstOrDefaultAsync();

            if (toEdit == null)
            {
                throw new ServicesException("Service not found", 404);
            }

            await servicesCollection.ReplaceOneAsync(s => s.Id == service.Id, service);

            return toEdit;
        }


        public async Task<Specialization> Add(Specialization spec)
        {
            spec.Id = Guid.NewGuid();

            var specCollection = _dbContext.GetCollection<Specialization>("Specializations");

            await specCollection.InsertOneAsync(spec);

            return spec;
        }

        public async Task<Specialization> Update(Specialization spec)
        {
            var specCollection = _dbContext.GetCollection<Specialization>("Specializations");

            var toEdit = await specCollection.Find(s => s.Id == spec.Id).FirstOrDefaultAsync();

            if (toEdit == null)
            {
                throw new ServicesException("Specialization not found", 404);
            }

            await specCollection.ReplaceOneAsync(s => s.Id == spec.Id, spec);

            return toEdit;
        }

        async Task<Specialization> ISpecializationDBService.Delete(Guid id)
        {
            var specCollection = _dbContext.GetCollection<Specialization>("Specializations");

            var spec = await specCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (spec == null)
            {
                throw new ServicesException("Specialization not found", 404);
            }

            await specCollection.DeleteOneAsync(s => s.Id == id);

            return spec;
        }

        async Task<Specialization> ISpecializationDBService.Get(Guid id)
        {
            var specializationsCollection = _dbContext.GetCollection<Specialization>("Specializations");

            var specialization = await specializationsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (specialization == null)
            {
                throw new ServicesException("Specialization not found", 404);
            }

            return specialization;
        }

        async Task<IEnumerable<Specialization>> ISpecializationDBService.Get(int page, int pageSize)
        {
            var servicesCollection = _dbContext.GetCollection<Specialization>("Specializations");

            FindOptions<Specialization> options = new FindOptions<Specialization>();
            options.Skip = (page - 1) * pageSize;
            options.Limit = pageSize;

            using (var cursor = await servicesCollection.FindAsync(FilterDefinition<Specialization>.Empty, options))
            {
                return await cursor.ToListAsync();
            }
        }

        async Task<Category> ICategoryDBService.Get(Guid id)
        {
            var categoriesCollection = _dbContext.GetCollection<Category>("Categories");

            var category = await categoriesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (category == null)
            {
                throw new ServicesException("Category not found", 404);
            }

            return category;
        }

        public async Task<IEnumerable<Category>> Get(int page, int pageSize)
        {
            var servicesCollection = _dbContext.GetCollection<Category>("Categories");

            FindOptions<Category> options = new FindOptions<Category>();
            options.Skip = (page - 1) * pageSize;
            options.Limit = pageSize;

            using (var cursor = await servicesCollection.FindAsync(FilterDefinition<Category>.Empty, options))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<Category> Add(Category category)
        {
            category.Id = Guid.NewGuid();

            var specCollection = _dbContext.GetCollection<Category>("Categories");

            await specCollection.InsertOneAsync(category);

            return category;
        }

        public async Task<Category> Update(Category category)
        {
            var specCollection = _dbContext.GetCollection<Category>("Categories");

            var toEdit = await specCollection.Find(s => s.Id == category.Id).FirstOrDefaultAsync();

            if (toEdit == null)
            {
                throw new ServicesException("Category not found", 404);
            }

            await specCollection.ReplaceOneAsync(s => s.Id == category.Id, category);

            return toEdit;
        }

        async Task<Category> ICategoryDBService.Delete(Guid id)
        {
            var categoryCollection = _dbContext.GetCollection<Category>("Categories");

            var category = await categoryCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (category == null)
            {
                throw new ServicesException("Category not found", 404);
            }

            await categoryCollection.DeleteOneAsync(s => s.Id == id);

            return category;
        }
    }
}
