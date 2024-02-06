using AutoMapper;
using CommonData.Constants;
using CommonData.Exceptions;
using DocumentsService.Domain.Interfaces;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using File = DocumentsService.Domain.Entities.File;

namespace DocumentsService.Infrastructure.Services
{
    public class MongoRepository : IFilesRepository
    {
        private readonly IMongoDatabase _dbContext;
        //private readonly IPublishEndpoint _publishEndpoint;
        //private readonly IMapper _mapper;

        public MongoRepository(IMongoDatabase dbContext)
        {
            _dbContext = dbContext;
            //_publishEndpoint = publishEndpoint;
            //_mapper = mapper;
        }

        public async Task<Guid> Add(File document)
        {
            document.Id = Guid.NewGuid();

            var filesCollection = _dbContext.GetCollection<File>("Files");

            await filesCollection.InsertOneAsync(document);

            return document.Id;
        }

        public async Task Delete(Guid id)
        {
            var filesCollection = _dbContext.GetCollection<File>("Files");

            await filesCollection.DeleteOneAsync(f => f.Id == id);

            return;
        }

        public async Task<File> Get(Guid id)
        {
            var filesCollection = _dbContext.GetCollection<File>("Files");

            var file = await filesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (file == null)
            {
                throw new InnoClinicException("File not found", 404);
            }

            return file;
        }

        public async Task<IEnumerable<File>> Get(int page, int pageSize, string? name, Guid? owner)
        {
            var filesCollection = _dbContext.GetCollection<File>("Files");

            FindOptions<File> options = new()
            {
                Skip = (page - 1) * pageSize,
                Limit = pageSize
            };

            var filterBuilder = Builders<File>.Filter;
            FilterDefinition<File> filter = filterBuilder.Empty;

            if (name != null && !name.IsNullOrEmpty())
            {
                filter &= filterBuilder.Regex("Name", new BsonRegularExpression(name));
            }
            if (owner != null)
            {
                filter &= filterBuilder.Eq("OwnerId", owner);
            }

            using (var cursor = await filesCollection.FindAsync(filter, options))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<Guid> Rename(Guid id, string newName)
        {
            var filesCollection = _dbContext.GetCollection<File>("Files");

            var file = await filesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (file == null)
            {
                throw new InnoClinicException("File not found", 404);
            }

            var definition = Builders<File>.Update.Set("Name", newName);

            await filesCollection.UpdateOneAsync(s => s.Id == id, definition);

            return file.Id;
        }

        public async Task<Guid> Update(File document)
        {
            var filesCollection = _dbContext.GetCollection<File>("Files");

            var file = await filesCollection.Find(s => s.Id == document.Id).FirstOrDefaultAsync();

            if (file == null)
            {
                throw new InnoClinicException("File not found", 404);
            }

            await filesCollection.ReplaceOneAsync(s => s.Id == document.Id, document);

            return document.Id;
        }
    }
}
