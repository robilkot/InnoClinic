using ProfilesService.Data;
using ProfilesService.Models;
using ProfilesService.Exceptions;

namespace ProfilesService.Services
{
    public class DbService
    {
        private readonly ProfilesDbContext _dbContext;
        public DbService(ProfilesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
    }
}
