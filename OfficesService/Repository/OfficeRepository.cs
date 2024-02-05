using AutoMapper;
using CommonData.Exceptions;
using CommonData.Messages;
using Dapper;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficesService.Data.Models;
using System.Data;
using System.Data.Common;
using Z.Dapper.Plus;
using static Dapper.SqlMapper;

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

            try
            {
                var result = await _connection.ExecuteAsync("INSERT INTO Offices (Id, Address, RegistryPhoneNumber, Active, ImageId) VALUES (@Id, @Address, @RegistryPhoneNumber, @Active, @ImageId)", entity);
            }
            catch (DbException dbEx)
            {
                throw new InnoClinicException(dbEx.Message, 500);
            }

            return entity;
        }

        public async Task Delete(Guid id)
        {
            var toDelete = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { Id = id });

            if (toDelete == null)
            {
                throw new InnoClinicException("Office not found", 404);
            }

            try
            {
                var result = await _connection.ExecuteAsync("DELETE FROM Offices WHERE Id = @Id", new { Id = id });
            }
            catch (DbException dbEx)
            {
                throw new InnoClinicException(dbEx.Message, 500);
            }

            await _publishEndpoint.Publish(new OfficeDelete() { Id = id });

            return;
        }

        public async Task<DbOfficeModel> Get(Guid id)
        {
            DbOfficeModel? office;

            try
            {
                office = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { Id = id });
            }
            catch (DbException dbEx)
            {
                throw new InnoClinicException(dbEx.Message, 500);
            }

            if (office == null)
            {
                throw new InnoClinicException("Office not found", 404);
            }

            return office;
        }

        public async Task<IEnumerable<DbOfficeModel>> GetAll()
        {
            IEnumerable<DbOfficeModel> offices;

            try
            {
                offices = await _connection.QueryAsync<DbOfficeModel>("SELECT * FROM Offices");
            }
            catch (DbException dbEx)
            {
                throw new InnoClinicException(dbEx.Message, 500);
            }

            return offices;
        }

        public async Task<DbOfficeModel> Update(DbOfficeModel entity)
        {
            DbOfficeModel? toEdit;

            try
            {
                toEdit = await _connection.QuerySingleOrDefaultAsync<DbOfficeModel>("SELECT * FROM Offices WHERE Id = @Id", new { entity.Id });
            }
            catch (DbException dbEx)
            {
                throw new InnoClinicException(dbEx.Message, 500);
            }

            if (toEdit == null)
            {
                throw new InnoClinicException("Office not found", 404);
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

            try
            {
                await _connection.ExecuteAsync(sql, parameters);
            }
            catch (DbException dbEx)
            {
                throw new InnoClinicException(dbEx.Message, 500);
            }

            // publish message to other services
            await _publishEndpoint.Publish(_mapper.Map<OfficeUpdate>(entity));

            return entity;
        }

        public void Init()
        {
            _connection.CreateTable<DbOfficeModel>();
        }
    }
}
