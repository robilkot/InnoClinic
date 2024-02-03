using AppointmentsService.Services;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class ServiceUpdateConsumer : IConsumer<ServiceUpdate>
    {
        private readonly DbService _dbService;
        public ServiceUpdateConsumer(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<ServiceUpdate> context)
        {
            Log.Debug("Service is being updated due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, null, context.Message.Id, null, null, null);

            foreach (var entity in appointmentsToEdit)
            {
                entity.ServiceName = context.Message.Name;

                await _dbService.UpdateAppointment(entity);
            };
        }
    }
}
