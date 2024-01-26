using AutoMapper;
using CommonData.Messages;
using MassTransit;
using OfficesService.Data.Models;
using OfficesService.Repository;
using Serilog;

namespace OfficesService.Consumers
{
    public class OfficeRequestConsumer : IConsumer<OfficeRequest>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<DbOfficeModel> _repository;
        public OfficeRequestConsumer(IMapper mapper, IRepository<DbOfficeModel> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<OfficeRequest> context)
        {
            Log.Debug("Office {@id} is being requested from outer service.", context.Message.Id);

            var requestedOffice = await _repository.Get(context.Message.Id);

            await context.RespondAsync(_mapper.Map<OfficeUpdate>(requestedOffice));
        }
    }
}
