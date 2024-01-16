using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Exceptions;
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
        [Authorize("patients.edit")]
        public async Task<ActionResult<IEnumerable<ClientPatientModel>>> GetPatients([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? name)
        {
            IEnumerable<ClientPatientModel> clientPatients;

            var dbPatients = await _dbService.GetPatients(pageNumber, pageSize, name);

            clientPatients = _mapper.Map<IEnumerable<ClientPatientModel>>(dbPatients);

            return new(clientPatients);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientPatientModel>> GetPatient(Guid id)
        {
            var dbPatient = await _dbService.GetPatient(id);

            if (!User.IsInRole("receptionist") && !CurrentUserIsPatient(dbPatient.AccountId))
            {
                throw new ProfilesException("Forbidden", 403);
            }

            var clientPatient = _mapper.Map<ClientPatientModel>(dbPatient);

            return clientPatient;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize("patients.edit")]
        public async Task<ActionResult<ClientPatientModel>> DeletePatient(Guid id)
        {
            var dbPatient = await _dbService.DeletePatient(id);

            Log.Information("Patient deleted => {@dbPatient}", dbPatient);

            var clientPatient = _mapper.Map<ClientPatientModel>(dbPatient);

            return clientPatient;
        }

        [HttpPost]
        [Authorize("patients.edit")]
        public async Task<ActionResult<ClientPatientModel>> CreatePatient([FromBody] ClientPatientModel patient)
        {
            var dbPatient = _mapper.Map<DbPatientModel>(patient);

            var addedPatient = await _dbService.AddPatient(dbPatient);

            Log.Information("Patient created => {@addedPatient}", addedPatient);

            var clientPatient = _mapper.Map<ClientPatientModel>(addedPatient);

            return clientPatient;
        }

        [HttpPut]
        public async Task<ActionResult<ClientPatientModel>> UpdatePatient([FromBody] ClientPatientModel patient)
        {
            var dbPatient = _mapper.Map<DbPatientModel>(patient);

            if (User.IsInRole("receptionist") || CurrentUserIsPatient(dbPatient.AccountId))
            {
                var updatedPatient = await _dbService.UpdatePatient(dbPatient);

                Log.Information("Patient updated => {@updatedPatient}", updatedPatient);

                var clientPatient = _mapper.Map<ClientPatientModel>(updatedPatient);

                return clientPatient;
            }
            else
            {
                throw new ProfilesException("Forbidden", 403);
            }
        }

        private bool CurrentUserIsPatient(Guid patientAccountId)
        {
            return User.IsInRole("patient") && patientAccountId.ToString() == User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }
    }
}