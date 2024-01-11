using Microsoft.EntityFrameworkCore;
using OfficesService.Data;
using OfficesService.Models;
using OfficesService.Exceptions;

namespace OfficesService.Services
{
    public class DbService
    {
        private readonly OfficesDbContext _dbContext;
        public DbService(OfficesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DbOfficeModel>> GetOffices()
        {
            // todo: problem with image loading when using asnotracking()
            IEnumerable<DbOfficeModel> offices = await _dbContext.Offices.ToListAsync();

            return offices;
        }
        public async Task<DbOfficeModel> GetOffice(Guid id)
        {
            var office = await _dbContext.Offices.FirstOrDefaultAsync(o => o.Id == id);

            if(office == null)
            {
                throw new OfficesException("Office not found", 404);
            }

            return office;
        }

        public async Task<DbOfficeModel> DeleteOffice(Guid id)
        {
            var office = await _dbContext.Offices.FirstOrDefaultAsync(o => o.Id == id);

            if (office == null)
            {
                throw new OfficesException("Office not found", 404);
            }

            _dbContext.Offices.Remove(office);

            await _dbContext.SaveChangesAsync();

            return office;
        }

        public async Task<DbOfficeModel> AddOffice(DbOfficeModel office)
        {
            office.Id = Guid.NewGuid();

            _dbContext.Offices.Add(office);

            await _dbContext.SaveChangesAsync();

            return office;
        }

        public async Task<DbOfficeModel> UpdateOffice(DbOfficeModel office)
        {
            var toEdit = await _dbContext.Offices.FirstOrDefaultAsync(o => o.Id == office.Id);

            if (toEdit == null)
            {
                throw new OfficesException("Office not found", 404);
            }

            _dbContext.Entry(toEdit).CurrentValues.SetValues(office);

            // todo: is photo being updated? check image models for client and db

            await _dbContext.SaveChangesAsync();

            return toEdit;
        }
    }
}
