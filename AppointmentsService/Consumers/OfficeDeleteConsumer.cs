using AppointmentsService.Interfaces;
using AutoMapper;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class OfficeDeleteConsumer : IConsumer<OfficeDelete>
    {
        private readonly IDbService _dbService;
        public OfficeDeleteConsumer(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<OfficeDelete> context)
        {
            Log.Debug("Office and its appointments is being deleted due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, null, null, null, context.Message.Id, null);

            foreach (var entity in appointmentsToEdit)
            {
                await _dbService.DeleteAppointment(entity.Id);
            };
        }
    }
}
