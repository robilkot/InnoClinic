using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Interfaces;
using ServicesService.Presentation.Models;

namespace ServicesService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("receptionists")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceDBService _dbService;
        private readonly IValidator<ClientServiceModel> _validator;
        private readonly IMapper _mapper;
        public ServicesController(IServiceDBService dbService, IMapper mapper, IValidator<ClientServiceModel> validator)
        {
            _dbService = dbService;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ClientServiceModel>>> GetServices([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] IEnumerable<Guid>? categories)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest();
            }

            var dbServices = await _dbService.Get(pageNumber, pageSize, categories);

            var clientServices = _mapper.Map<IEnumerable<ClientServiceModel>>(dbServices);

            return new(clientServices);
        }

        [HttpGet("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ClientServiceModel>> GetService(Guid id)
        {
            var dbService = await _dbService.Get(id);

            var clientService = _mapper.Map<ClientServiceModel>(dbService);

            return clientService;
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<ClientServiceModel>> DeleteService(Guid id)
        {
            await _dbService.Delete(id);

            Log.Information("Service deleted: {@dbService}", id);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ClientServiceModel>> CreateService([FromBody] ClientServiceModel service)
        {
            var result = await _validator.ValidateAsync(service);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);

                return ValidationProblem(ModelState);
            }

            var dbPatient = _mapper.Map<Service>(service);

            var addedService = await _dbService.Add(dbPatient);

            Log.Information("Service created: {@addedService}", (addedService.Id, addedService.Name));

            var clientService = _mapper.Map<ClientServiceModel>(addedService);

            return clientService;
        }

        [HttpPut]
        public async Task<ActionResult<ClientServiceModel>> UpdateService([FromBody] ClientServiceModel service)
        {
            var result = await _validator.ValidateAsync(service);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);

                return ValidationProblem(ModelState);
            }

            var dbPatient = _mapper.Map<Service>(service);

            var updatedService = await _dbService.Update(dbPatient);

            Log.Information("Service updated: {@updatedService}", (updatedService.Id, updatedService.Name));

            var clientService = _mapper.Map<ClientServiceModel>(updatedService);

            return clientService;
        }
    }
}
