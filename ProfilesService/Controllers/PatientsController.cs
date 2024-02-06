using AutoMapper;
using CommonData.Constants;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Data.Models;
using ProfilesService.Models;
using ProfilesService.Services;
using Serilog;

namespace ProfilesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly DbService _dbService;
        private readonly IMapper _mapper;
        public PatientsController(DbService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientPatientModel>>> GetPatients([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? name)
        {
            if (!User.IsInRole(Roles.Receptionist) && !User.IsInRole(Roles.Doctor))
            {
                return Forbid();
            }

            var dbPatients = await _dbService.GetPatients(pageNumber, pageSize, name);

            var clientPatients = _mapper.Map<IEnumerable<ClientPatientModel>>(dbPatients);

            return new(clientPatients);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientPatientModel>> GetPatient(Guid id)
        {
            var dbPatient = await _dbService.GetPatient(id);

            if (!User.IsInRole(Roles.Receptionist) && !User.IsInRole(Roles.Doctor) && !CurrentUserIsPatient(dbPatient.AccountId))
            {
                return Forbid();
            }

            var clientPatient = _mapper.Map<ClientPatientModel>(dbPatient);

            return clientPatient;
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeletePatient(Guid id)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            await _dbService.DeletePatient(id);

            Log.Information("Patient deleted: {@patientId}", id);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ClientPatientModel>> CreatePatient([FromBody] ClientPatientModel patient)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbPatient = _mapper.Map<DbPatientModel>(patient);

            var addedPatient = await _dbService.AddPatient(dbPatient);

            Log.Information("Patient created => {@addedPatient}", addedPatient);

            var clientPatient = _mapper.Map<ClientPatientModel>(addedPatient);

            return clientPatient;
        }

        [HttpPut]
        public async Task<ActionResult<ClientPatientModel>> UpdatePatient([FromBody] ClientPatientModel patient)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbPatient = _mapper.Map<DbPatientModel>(patient);

            if (!User.IsInRole(Roles.Receptionist) && !CurrentUserIsPatient(dbPatient.AccountId))
            {
                return Forbid();
            }
        
            var updatedPatient = await _dbService.UpdatePatient(dbPatient);

            Log.Information("Patient updated: {@updatedPatient}", (updatedPatient.Id, updatedPatient.LastName));

            var clientPatient = _mapper.Map<ClientPatientModel>(updatedPatient);

            return clientPatient;
        }

        private bool CurrentUserIsPatient(Guid patientAccountId)
        {
            return User.IsInRole(Roles.Patient) && patientAccountId.ToString() == User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }
    }
}