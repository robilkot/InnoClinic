using AutoMapper;
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
        [Authorize("receptionists.edit")]
        public async Task<ActionResult<IEnumerable<ClientReceptionistModel>>> GetReceptionists([FromQuery] int pageNumber,
                                                                                               [FromQuery] int pageSize,
                                                                                               [FromQuery] IEnumerable<Guid> officesId,
                                                                                               [FromQuery] string? name)
        {
            IEnumerable<ClientReceptionistModel> clientReceptionists;

            var dbReceptionists = await _dbService.GetReceptionists(pageNumber, pageSize, officesId, name);

            clientReceptionists = _mapper.Map<IEnumerable<ClientReceptionistModel>>(dbReceptionists);

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
        [Authorize("receptionists.edit")]
        public async Task<ActionResult> DeleteReceptionist(Guid id)
        {
            await _dbService.DeleteReceptionist(id);

            Log.Information("Receptionist deleted => {@dbReceptionist}", id);

            return NoContent();
        }

        [HttpPost]
        [Authorize("receptionists.edit")]
        public async Task<ActionResult<ClientReceptionistModel>> CreateReceptionist([FromBody] ClientReceptionistModel receptionist)
        {
            var dbReceptionist = _mapper.Map<DbReceptionistModel>(receptionist);

            var addedReceptionist = await _dbService.AddReceptionist(dbReceptionist);

            Log.Information("Receptionist created => {@addedReceptionist}", addedReceptionist);

            var clientReceptionist = _mapper.Map<ClientReceptionistModel>(addedReceptionist);

            return clientReceptionist;
        }

        [HttpPut]
        public async Task<ActionResult<ClientReceptionistModel>> UpdateReceptionist([FromBody] ClientReceptionistModel receptionist)
        {
            var dbReceptionist = _mapper.Map<DbReceptionistModel>(receptionist);

            var updatedReceptionist = await _dbService.UpdateReceptionist(dbReceptionist);

            Log.Information("Receptionist updated => {@updatedReceptionist}", updatedReceptionist);

            var clientReceptionist = _mapper.Map<ClientReceptionistModel>(updatedReceptionist);

            return clientReceptionist;

        }
    }
}