using AutoMapper;
using CommonData.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Data.Models;
using ProfilesService.Models;
using ProfilesService.Services;
using Serilog;

namespace ProfilesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("receptionists")]
    public class ReceptionistsController : ControllerBase
    {
        private readonly DbService _dbService;
        private readonly IMapper _mapper;
        public ReceptionistsController(DbService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientReceptionistModel>>> GetReceptionists([FromQuery] int pageNumber,
                                                                                               [FromQuery] int pageSize,
                                                                                               [FromQuery] IEnumerable<Guid> officesId,
                                                                                               [FromQuery] string? name)
        {
            if(!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbReceptionists = await _dbService.GetReceptionists(pageNumber, pageSize, officesId, name);

            var clientReceptionists = _mapper.Map<IEnumerable<ClientReceptionistModel>>(dbReceptionists);

            return new(clientReceptionists);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientReceptionistModel>> GetReceptionist(Guid id)
        {
            var dbReceptionist = await _dbService.GetReceptionist(id);

            var clientReceptionist = _mapper.Map<ClientReceptionistModel>(dbReceptionist);

            return clientReceptionist;
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteReceptionist(Guid id)
        {
            await _dbService.DeleteReceptionist(id);

            Log.Information("Receptionist deleted: {@dbReceptionist}", id);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ClientReceptionistModel>> CreateReceptionist([FromBody] ClientReceptionistModel receptionist)
        {
            var dbReceptionist = _mapper.Map<DbReceptionistModel>(receptionist);

            var addedReceptionist = await _dbService.AddReceptionist(dbReceptionist);

            Log.Information("Receptionist created: {@addedReceptionist}", (addedReceptionist.Id, addedReceptionist.LastName));

            var clientReceptionist = _mapper.Map<ClientReceptionistModel>(addedReceptionist);

            return clientReceptionist;
        }

        [HttpPut]
        public async Task<ActionResult<ClientReceptionistModel>> UpdateReceptionist([FromBody] ClientReceptionistModel receptionist)
        {
            var dbReceptionist = _mapper.Map<DbReceptionistModel>(receptionist);

            var updatedReceptionist = await _dbService.UpdateReceptionist(dbReceptionist);

            Log.Information("Receptionist updated: {@updatedReceptionist}", (updatedReceptionist.Id, updatedReceptionist.LastName));

            var clientReceptionist = _mapper.Map<ClientReceptionistModel>(updatedReceptionist);

            return clientReceptionist;
        }
    }
}