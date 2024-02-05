using AppointmentsService.Interfaces;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class PatientDeleteConsumer : IConsumer<PatientDelete>
    {
        private readonly IDbService _dbService;
        public PatientDeleteConsumer(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<PatientDelete> context)
        {
            Log.Debug("Patient and his appointments are being deleted due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, null, null, null, null, context.Message.Id);

            foreach (var entity in appointmentsToEdit)
            {
                await _dbService.DeleteAppointment(entity.Id);
            };
        }
    }
}
