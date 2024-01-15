using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Models;
using ProfilesService.Services;
using Serilog;

namespace ProfilesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly DbService _dbService;
        private readonly IMapper _mapper;
        public DoctorsController(DbService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        // Enumerable params to sort doctors by different fields
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDoctorModel>>> GetDoctors(IEnumerable<Guid>? specializations, IEnumerable<Guid>? offices, string? doctorName)
        {
            //var dbOffices = await _dbService.GetOffices();

            //var clientOffices = _mapper.Map<IEnumerable<ClientOfficeModel>>(dbOffices);

            //return new(clientOffices);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientDoctorModel>> GetDoctor(Guid id)
        {
            //var dbOffice = await _dbService.GetOffice(id);

            //var clientOffice = _mapper.Map<ClientOfficeModel>(dbOffice);

            //return clientOffice;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize("doctors.edit")]
        public async Task<ActionResult<ClientDoctorModel>> DeleteDoctor(Guid id)
        {
            //var dbOffice = await _dbService.DeleteOffice(id);

            //Log.Information("Office deleted => {@dbOffice}", dbOffice);

            //var clientOffice = _mapper.Map<ClientOfficeModel>(dbOffice);

            //return clientOffice;
        }

        [HttpPost]
        [Authorize("doctors.edit")]
        public async Task<ActionResult<ClientDoctorModel>> CreateDoctor([FromBody] ClientDoctorModel doctor)
        {
            //var dbOffice = _mapper.Map<DbOfficeModel>(office);

            //var addedOffice = await _dbService.AddOffice(dbOffice);

            //Log.Information("Office created => {@addedOffice}", addedOffice);

            //var clientOffice = _mapper.Map<ClientOfficeModel>(addedOffice);

            //return clientOffice;
        }

        [HttpPut]
        [Authorize("doctors.edit")]
        public async Task<ActionResult<ClientDoctorModel>> UpdateDoctor([FromBody] ClientDoctorModel doctor)
        {
            //var dbOffice = _mapper.Map<DbOfficeModel>(office);

            //var updatedOffice = await _dbService.UpdateOffice(dbOffice);

            //Log.Information("Office updated => {@addedOffice}", updatedOffice);

            //var clientOffice = _mapper.Map<ClientOfficeModel>(updatedOffice);

            //return clientOffice;
        }
    }
}