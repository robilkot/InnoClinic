using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Interfaces;

namespace ServicesService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationDBService _dbService;
        private readonly IMapper _mapper;
        private readonly IValidator<ClientSpecializationModel> _validator;
        public SpecializationsController(ISpecializationDBService dbService, IMapper mapper, IValidator<ClientSpecializationModel> validator)
        {
            _dbService = dbService;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientSpecializationModel>>> GetSpecializations([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var dbSpecs = await _dbService.Get(pageNumber, pageSize);

            var clientSpecs = _mapper.Map<IEnumerable<ClientSpecializationModel>>(dbSpecs);

            return new(clientSpecs);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientSpecializationModel>> GetSpecialization(Guid id)
        {
            var dbSpec = await _dbService.Get(id);

            var clientSpec = _mapper.Map<ClientSpecializationModel>(dbSpec);

            return clientSpec;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize("specializations.edit")]
        public async Task<ActionResult<ClientSpecializationModel>> DeleteSpecialization(Guid id)
        {
            var dbSpec = await _dbService.Delete(id);

            Log.Information("Specialization deleted => {@dbSpec}", dbSpec);

            var clientSpec = _mapper.Map<ClientSpecializationModel>(dbSpec);

            return clientSpec;
        }

        [HttpPost]
        [Authorize("specializations.edit")]
        public async Task<ActionResult<ClientSpecializationModel>> CreateService([FromBody] ClientSpecializationModel spec)
        {
            var result = await _validator.ValidateAsync(spec);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);

                return ValidationProblem(ModelState);
            }

            var dbSpec = _mapper.Map<Specialization>(spec);

            var addedSpec = await _dbService.Add(dbSpec);

            Log.Information("Specialization created => {@addedSpec}", addedSpec);

            var clientSpec = _mapper.Map<ClientSpecializationModel>(addedSpec);

            return clientSpec;
        }

        [HttpPut]
        [Authorize("specializations.edit")]
        public async Task<ActionResult<ClientSpecializationModel>> UpdateService([FromBody] ClientSpecializationModel spec)
        {
            var result = await _validator.ValidateAsync(spec);

            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);

                return ValidationProblem(ModelState);
            }

            var dbSpec = _mapper.Map<Specialization>(spec);

            var updatedSpec = await _dbService.Update(dbSpec);

            Log.Information("Specialization updated => {@updatedSpec}", updatedSpec);

            var clientSpec = _mapper.Map<ClientSpecializationModel>(updatedSpec);

            return clientSpec;
        }
    }
}
