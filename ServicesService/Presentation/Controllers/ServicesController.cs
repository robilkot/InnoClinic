using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Interfaces;
using ServicesService.Infrastructure.Services;
using ServicesService.Presentation.Models;

namespace ServicesService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceDBService _dbService;
        private readonly IMapper _mapper;
        public ServicesController(IServiceDBService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientServiceModel>>> GetServices([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] IEnumerable<Guid>? categories)
        {
            IEnumerable<ClientServiceModel> clientServices;

            var dbServices = await _dbService.Get(pageNumber, pageSize, categories);

            clientServices = _mapper.Map<IEnumerable<ClientServiceModel>>(dbServices);

            return new(clientServices);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientServiceModel>> GetService(Guid id)
        {
            var dbService = await _dbService.Get(id);

            var clientService = _mapper.Map<ClientServiceModel>(dbService);

            return clientService;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize("services.edit")]
        public async Task<ActionResult<ClientServiceModel>> DeleteService(Guid id)
        {
            var dbService = await _dbService.Delete(id);

            Log.Information("Service deleted => {@dbService}", dbService);

            var clientService = _mapper.Map<ClientServiceModel>(dbService);

            return clientService;
        }

        [HttpPost]
        [Authorize("services.edit")]
        public async Task<ActionResult<ClientServiceModel>> CreateService([FromBody] ClientServiceModel service)
        {
            var dbPatient = _mapper.Map<Service>(service);

            var addedService = await _dbService.Add(dbPatient);

            Log.Information("Service created => {@addedService}", addedService);

            var clientService = _mapper.Map<ClientServiceModel>(addedService);

            return clientService;
        }

        [HttpPut]
        [Authorize("services.edit")]
        public async Task<ActionResult<ClientServiceModel>> UpdateService([FromBody] ClientServiceModel service)
        {
            var dbPatient = _mapper.Map<Service>(service);

            var updatedService = await _dbService.Update(dbPatient);

            Log.Information("Service updated => {@updatedService}", updatedService);

            var clientService = _mapper.Map<ClientServiceModel>(updatedService);

            return clientService;
        }
    }
}
