using CommonData.Messages;
using MassTransit;
using ProfilesService.Services;
using Serilog;

namespace ProfilesService.Consumers
{
    public class OfficeUpdateConsumer : IConsumer<OfficeUpdate>
    {
        private readonly DbService _dbService;
        public OfficeUpdateConsumer(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<OfficeUpdate> context)
        {
            Log.Debug("Office is being updated due to outer message.");

            var doctorsToEdit = await _dbService.GetDoctors(0, 0, null, null, null, null);

            foreach (var entity in doctorsToEdit)
            {
                if (entity.OfficeId == context.Message.Id)
                {
                    entity.OfficeAddress = context.Message.Address;
                }
                await _dbService.UpdateDoctor(entity);
            };

            var receptionistsToEdit = await _dbService.GetReceptionists(0, 0, null);

            foreach (var entity in receptionistsToEdit)
            {
                if (entity.OfficeId == context.Message.Id)
                {
                    entity.OfficeAddress = context.Message.Address;
                }
                await _dbService.UpdateReceptionist(entity);
            };
        }
    }
}
