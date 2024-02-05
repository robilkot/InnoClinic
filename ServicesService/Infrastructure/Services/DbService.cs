using AutoMapper;
using CommonData.Constants;
using CommonData.Exceptions;
using CommonData.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Interfaces;

namespace ServicesService.Infrastructure.Services
{
    public class DbService : IServiceDBService, ISpecializationDBService, ICategoryDBService
    {
        private readonly IMongoDatabase _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public DbService(IMongoDatabase dbContext, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        public async Task<Service> Add(Service service)
        {
            service.Id = Guid.NewGuid();

            var categoriesCollection = _dbContext.GetCollection<Category>("Categories");

            if (service.CategoryId == null)
            {
                service.TimeSlotSize = TimeSlotsConstants.DefaultTimeSlotSize;
            }
            else
            {
                var serviceCategory = await categoriesCollection.Find(c => c.Id == service.CategoryId).FirstOrDefaultAsync();

                if (serviceCategory == null)
                {
                    throw new InnoClinicException("Can't find specified service category", 404);
                }
                else
                {
                    service.TimeSlotSize = serviceCategory.TimeSlotSize;
                }
            }

            var specializationsCollection = _dbContext.GetCollection<Specialization>("Specializations");

            var serviceSpec = await specializationsCollection.Find(c => c.Id == service.SpecializationId).FirstOrDefaultAsync();

            if (serviceSpec == null)
            {
                throw new InnoClinicException("Can't find specified service specialization", 404);
            }

            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            await servicesCollection.InsertOneAsync(service);

            return service;
        }

        public async Task Delete(Guid id)
        {
            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            var service = await servicesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (service == null)
            {
                throw new InnoClinicException("Service not found", 404);
            }

            await servicesCollection.DeleteOneAsync(s => s.Id == id);

            await _publishEndpoint.Publish(new ServiceDelete() { Id = id });

            return;
        }

        public async Task<Service> Get(Guid id)
        {
            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            var service = await servicesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (service == null)
            {
                throw new InnoClinicException("Service not found", 404);
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
                filter &= filterBuilder.In("CategoryId", categories);
            }

            FindOptions<Service> options = new()
            {
                Skip = (page - 1) * pageSize,
                Limit = pageSize
            };

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
                throw new InnoClinicException("Service not found", 404);
            }

            // Do not change timeslotsize to new value
            service.TimeSlotSize = toEdit.TimeSlotSize;

            await servicesCollection.ReplaceOneAsync(s => s.Id == service.Id, service);

            Log.Debug("Publishing message to update changed service");

            var msg = _mapper.Map<ServiceUpdate>(service);

            await _publishEndpoint.Publish(msg);

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
                throw new InnoClinicException("Specialization not found", 404);
            }

            await specCollection.ReplaceOneAsync(s => s.Id == spec.Id, spec);

            return toEdit;
        }

        async Task ISpecializationDBService.Delete(Guid id)
        {
            var specCollection = _dbContext.GetCollection<Specialization>("Specializations");

            var spec = await specCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (spec == null)
            {
                throw new InnoClinicException("Specialization not found", 404);
            }

            await specCollection.DeleteOneAsync(s => s.Id == id);


            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            UpdateDefinition<Service> updateDefinition = Builders<Service>.Update.Set(x => x.SpecializationId, null);

            var servicesToUpdate = await servicesCollection.UpdateManyAsync(x => x.SpecializationId == spec.Id, updateDefinition);

            await _publishEndpoint.Publish(new SpecializationDelete() { Id = id });

            return;
        }

        async Task<Specialization> ISpecializationDBService.Get(Guid id)
        {
            var specializationsCollection = _dbContext.GetCollection<Specialization>("Specializations");

            var specialization = await specializationsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (specialization == null)
            {
                throw new InnoClinicException("Specialization not found", 404);
            }

            return specialization;
        }

        async Task<IEnumerable<Specialization>> ISpecializationDBService.Get(int page, int pageSize)
        {
            var servicesCollection = _dbContext.GetCollection<Specialization>("Specializations");

            FindOptions<Specialization> options = new()
            {
                Skip = (page - 1) * pageSize,
                Limit = pageSize
            };

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
                throw new InnoClinicException("Category not found", 404);
            }

            return category;
        }

        public async Task<IEnumerable<Category>> Get(int page, int pageSize)
        {
            var servicesCollection = _dbContext.GetCollection<Category>("Categories");

            FindOptions<Category> options = new()
            {
                Skip = (page - 1) * pageSize,
                Limit = pageSize
            };

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
            var categoriesCollection = _dbContext.GetCollection<Category>("Categories");

            var toEdit = await categoriesCollection.Find(s => s.Id == category.Id).FirstOrDefaultAsync();

            if (toEdit == null)
            {
                throw new InnoClinicException("Category not found", 404);
            }

            await categoriesCollection.ReplaceOneAsync(s => s.Id == category.Id, category);


            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            UpdateDefinition<Service> updateDefinition = Builders<Service>.Update.Set("TimeSlotSize", category.TimeSlotSize);

            var updatedServices = await servicesCollection.UpdateManyAsync(x => x.CategoryId == category.Id, updateDefinition);

            var servicesToSynchronize = await servicesCollection.Find(x => x.CategoryId == category.Id).ToListAsync();

            Log.Debug("Publishing messages to update changed services");

            foreach (var svc in servicesToSynchronize)
            {
                var msg = _mapper.Map<ServiceUpdate>(svc);

                await _publishEndpoint.Publish(msg);
            }

            return category;
        }

        async Task ICategoryDBService.Delete(Guid id)
        {
            var categoryCollection = _dbContext.GetCollection<Category>("Categories");

            var category = await categoryCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (category == null)
            {
                throw new InnoClinicException("Category not found", 404);
            }

            await categoryCollection.DeleteOneAsync(s => s.Id == id);

            var servicesCollection = _dbContext.GetCollection<Service>("Services");

            UpdateDefinition<Service> updateDefinition = Builders<Service>.Update.Set(x => x.CategoryId, null);

            var servicesToUpdate = await servicesCollection.UpdateManyAsync(x => x.CategoryId == category.Id, updateDefinition);

            return;
        }
    }
}
