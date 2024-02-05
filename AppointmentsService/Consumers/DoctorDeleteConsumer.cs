using AppointmentsService.Interfaces;
using CommonData.Constants;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class DoctorDeleteConsumer : IConsumer<DoctorDelete>
    {
        private readonly IDbService _dbService;
        public DoctorDeleteConsumer(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<DoctorDelete> context)
        {
            Log.Debug("Doctor is being deleted due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, context.Message.Id, null, null, null, null);

            foreach (var entity in appointmentsToEdit)
            {
                await _dbService.DeleteAppointment(entity.Id);
            };
        }
    }
}
