using AutoMapper;
using CommonData.Messages;
using Dapper;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficesService.Data.Models;
using OfficesService.Exceptions;
using System.Data;
using Z.Dapper.Plus;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OfficesService.Repository
{
    public class OfficeRepository : IRepository<DbOfficeModel>
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public OfficeRepository(IConfiguration configuration, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            var connectionString = Environment.GetEnvironmentVariable("DbConnection") ?? configuration.GetConnectionString("DbConnection");
            _connection = new SqlConnection(connectionString);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<DbOfficeModel> Add(DbOfficeModel entity)
        {
            entity.Id = Guid.NewGuid();

            var result = await _connection.ExecuteAsync("INSERT INTO Offices (Id, Address, RegistryPhoneNumber, Active, ImageId) VALUES (@Id, @Address, @RegistryPhoneNumber, @Active, @ImageId)", entity);

            return result > 0 ? entity : throw new OfficesException("Couldn't add new office", 500);
        }

        public async Task<DbOfficeModel> Delete(Guid id)
        {
            var toDelete = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { Id = id });

            if (toDelete == null)
            {
                throw new OfficesException("Office not found", 404);
            }

            var result = await _connection.ExecuteAsync("DELETE FROM Offices WHERE Id = @Id", new { Id = id });

            return result > 0 ? toDelete : throw new OfficesException("Couldn't add new office", 500);
        }

        public async Task<DbOfficeModel> Get(Guid id)
        {
            var office = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { Id = id });

            if (office == null)
            {
                throw new OfficesException("Office not found", 404);
            }

            return office;
        }

        public async Task<IEnumerable<DbOfficeModel>> GetAll()
        {
            var offices = await _connection.QueryAsync<DbOfficeModel>("SELECT * FROM Offices");

            return offices;
        }

        public async Task<DbOfficeModel> Update(DbOfficeModel entity)
        {
            var toEdit = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { entity.Id });

            if (toEdit == null)
            {
                throw new OfficesException("Office not found", 404);
            }

            var sql = @"UPDATE Offices
					SET Address = @Address,
						RegistryPhoneNumber = @RegistryPhoneNumber,
						Active = @Active,
						ImageId = @ImageId
					WHERE Id = @Id";

            var parameters = new
            {
                Address = entity.Address,
                RegistryPhoneNumber = entity.RegistryPhoneNumber,
                Active = entity.Active,
                ImageId = entity.ImageId,
                Id = entity.Id
            };

            // todo: is photo being updated? check image models for client and db
            await _connection.ExecuteAsync(sql, parameters);

            // query edited office
            var editedOffice = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { entity.Id });

            // publish message to other services
            await _publishEndpoint.Publish(_mapper.Map<OfficeUpdate>(editedOffice));

            return editedOffice!;
        }

        public void Init()
        {
            _connection.CreateTable<DbOfficeModel>();
        }
    }
}
