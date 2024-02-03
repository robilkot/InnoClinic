using AutoMapper;
using CommonData.Messages;
using MassTransit;
using ProfilesService.Services;
using Serilog;

namespace OfficesService.Consumers
{
    public class DoctorRequestConsumer : IConsumer<DoctorRequest>
    {
        private readonly IMapper _mapper;
        private readonly DbService _dbService;
        public DoctorRequestConsumer(IMapper mapper, DbService dbService)
        {
            _mapper = mapper;
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<DoctorRequest> context)
        {
            Log.Debug("Doctor {@id} is being requested from outer service.", context.Message.Id);

            var requestedDoctor = await _dbService.GetDoctor(context.Message.Id);

            await context.RespondAsync(_mapper.Map<DoctorUpdate>(requestedDoctor));
        }
    }
}
