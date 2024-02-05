using CommonData.Constants;
using CommonData.Messages;
using MassTransit;
using ProfilesService.Services;
using Serilog;

namespace ProfilesService.Consumers
{
    public class OfficeDeleteConsumer : IConsumer<OfficeDelete>
    {
        private readonly DbService _dbService;
        public OfficeDeleteConsumer(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<OfficeDelete> context)
        {
            Log.Debug("Office is being updated due to outer message.");

            var doctorsToEdit = await _dbService.GetDoctors(0, 0, null, new List<Guid>(){ context.Message.Id }, null, null);

            foreach (var entity in doctorsToEdit)
            {
                entity.OfficeAddress = StringConstants.OfficeDeleted;

                await _dbService.UpdateDoctor(entity);
            };

            var receptionistsToEdit = await _dbService.GetReceptionists(0, 0, new List<Guid>() { context.Message.Id }, null);

            foreach (var entity in receptionistsToEdit)
            {
                entity.OfficeAddress = StringConstants.OfficeDeleted;

                await _dbService.UpdateReceptionist(entity);
            };
        }
    }
}
