using AppointmentsService.Services;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class PatientUpdateConsumer : IConsumer<PatientUpdate>
    {
        private readonly DbService _dbService;
        public PatientUpdateConsumer(DbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<PatientUpdate> context)
        {
            Log.Debug("Patient is being updated due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, null, null, null, null, context.Message.Id);

            foreach (var entity in appointmentsToEdit)
            {
                entity.PatientFirstName = context.Message.FirstName;
                entity.PatientMiddleName = context.Message.MiddleName;
                entity.PatientLastName = context.Message.LastName;
                
                await _dbService.UpdateAppointment(entity);
            };
        }
    }
}
