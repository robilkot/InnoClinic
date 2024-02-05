using AppointmentsService.Interfaces;
using CommonData.Messages;
using MassTransit;
using Serilog;

namespace AppointmentsService.Consumers
{
    public class DoctorUpdateConsumer : IConsumer<DoctorUpdate>
    {
        private readonly IDbService _dbService;
        public DoctorUpdateConsumer(IDbService dbService)
        {
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<DoctorUpdate> context)
        {
            Log.Debug("Doctor is being updated due to outer message.");

            var appointmentsToEdit = await _dbService.GetAppointments(0, 0, null, context.Message.Id, null, null, null, null);

            foreach (var entity in appointmentsToEdit)
            {
                entity.DoctorFirstName = context.Message.FirstName;
                entity.DoctorMiddleName = context.Message.MiddleName;
                entity.DoctorLastName = context.Message.LastName;

                await _dbService.UpdateAppointment(entity);
            };
        }
    }
}
