using AppointmentsService.Interfaces;
using AutoMapper;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class OfficeUpdateConsumer : IConsumer<OfficeUpdate>
    {
        private readonly IDbService _dbService;
        public OfficeUpdateConsumer(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<OfficeUpdate> context)
        {
            Log.Debug("Office is being updated due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, null, null, null, null, null);

            foreach (var entity in appointmentsToEdit)
            {
                if (entity.OfficeId == context.Message.Id)
                {
                    entity.OfficeAddress = context.Message.Address;
                }
                await _dbService.UpdateAppointment(entity);
            };
        }
    }
}
