using AutoMapper;
using CommonData.Constants;
using CommonData.Messages;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficesService.Data.Models;
using OfficesService.Models;
using OfficesService.Repository;
using Serilog;

namespace OfficesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OfficesController : ControllerBase
    {
        private readonly IRepository<DbOfficeModel> _officesRepository;
        private readonly IMapper _mapper;
        public OfficesController(IRepository<DbOfficeModel> dbService, IMapper mapper)
        {
            _officesRepository = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ClientOfficeModel>>> GetOffices()
        {
            var dbOffices = await _officesRepository.GetAll();

            var clientOffices = _mapper.Map<IEnumerable<ClientOfficeModel>>(dbOffices);

            return new(clientOffices);
        }

        [HttpGet("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ClientOfficeModel>> GetOffice(Guid id)
        {
            var dbOffice = await _officesRepository.Get(id);

            var clientOffice = _mapper.Map<ClientOfficeModel>(dbOffice);

            return clientOffice;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult> DeleteOffice(Guid id)
        {
            if(!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            await _officesRepository.Delete(id);

            Log.Information("Office deleted: Id = {@dbOffice}", id);

            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClientOfficeModel>> CreateOffice([FromBody] ClientOfficeModel office)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbOffice = _mapper.Map<DbOfficeModel>(office);

            var addedOffice = await _officesRepository.Add(dbOffice);

            Log.Information("Office created: {@addedOffice}", (addedOffice.Id, addedOffice.Address));

            var clientOffice = _mapper.Map<ClientOfficeModel>(addedOffice);

            return clientOffice;
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ClientOfficeModel>> UpdateOffice([FromBody] ClientOfficeModel office)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbOffice = _mapper.Map<DbOfficeModel>(office);

            var updatedOffice = await _officesRepository.Update(dbOffice);

            Log.Information("Office updated: {@updatedOffice}", (updatedOffice.Id, updatedOffice.Address));

            var clientOffice = _mapper.Map<ClientOfficeModel>(updatedOffice);

            return clientOffice;
        }
    }
}