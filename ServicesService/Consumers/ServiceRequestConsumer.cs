using AutoMapper;
using CommonData.Messages;
using MassTransit;
using Serilog;
using ServicesService.Domain.Interfaces;

namespace ServicesService.Consumers
{
    public class ServiceRequestConsumer : IConsumer<ServiceRequest>
    {
        private readonly IMapper _mapper;
        private readonly IServiceDBService _dbService;
        public ServiceRequestConsumer(IMapper mapper, IServiceDBService dbService)
        {
            _mapper = mapper;
            _dbService = dbService;
        }
        public async Task Consume(ConsumeContext<ServiceRequest> context)
        {
            Log.Debug("Service {@id} is being requested from outer service.", context.Message.Id);

            var requestedService = await _dbService.Get(context.Message.Id);

            await context.RespondAsync(_mapper.Map<ServiceUpdate>(requestedService));
        }
    }
}
