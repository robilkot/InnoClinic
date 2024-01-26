using AutoMapper;
using CommonData.Messages;
using MassTransit;
using Serilog;
using ServicesService.Domain.Interfaces;

namespace ServicesService.Consumers
{
    public class SpecializationRequestConsumer : IConsumer<SpecializationRequest>
    {
        private readonly IMapper _mapper;
        private readonly ISpecializationDBService _dbService;
        public SpecializationRequestConsumer(IMapper mapper, ISpecializationDBService dbService)
        {
            _mapper = mapper;
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<SpecializationRequest> context)
        {
            Log.Debug("Specialization {@id} is being requested from outer service.", context.Message.Id);

            var requestedSpec = await _dbService.Get(context.Message.Id);

            await context.RespondAsync(_mapper.Map<SpecializationUpdate>(requestedSpec));
        }
    }
}
