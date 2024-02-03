using AutoMapper;
using CommonData.Messages;
using MassTransit;
using ProfilesService.Services;
using Serilog;

namespace OfficesService.Consumers
{
    public class PatientRequestConsumer : IConsumer<PatientRequest>
    {
        private readonly IMapper _mapper;
        private readonly DbService _dbService;
        public PatientRequestConsumer(IMapper mapper, DbService dbService)
        {
            _mapper = mapper;
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<PatientRequest> context)
        {
            Log.Debug("Patient {@id} is being requested from outer service.", context.Message.Id);

            var requestedPatient = await _dbService.GetPatient(context.Message.Id);

            await context.RespondAsync(_mapper.Map<PatientUpdate>(requestedPatient));
        }
    }
}
