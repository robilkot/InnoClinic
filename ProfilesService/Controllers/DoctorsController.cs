using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Exceptions;
using ProfilesService.Models;
using ProfilesService.Services;
using Serilog;
using System.Numerics;

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
        public async Task<ActionResult<IEnumerable<ClientDoctorModel>>> GetDoctors([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] IEnumerable<Guid>? specializations, [FromQuery] IEnumerable<Guid>? offices,[FromQuery] string? doctorName, IEnumerable<DoctorStatusEnum>? status)
        {
            IEnumerable<ClientDoctorModel> clientDoctors;

            if(User.IsInRole("receptionist"))
            {
                var dbDoctors = await _dbService.GetDoctors(pageNumber, pageSize, specializations, offices, status, doctorName);
                
                clientDoctors = _mapper.Map<IEnumerable<ClientDoctorModel>>(dbDoctors);
            }
            else
            {
                var dbDoctors = await _dbService.GetDoctors(pageNumber, pageSize, specializations, offices, new List<DoctorStatusEnum>() { DoctorStatusEnum.AtWork }, doctorName);

                clientDoctors = _mapper.Map<IEnumerable<ClientDoctorModel>>(dbDoctors);

                foreach (var doctor in clientDoctors)
                {
                    doctor.DateOfBirth = null;
                    doctor.Email = null;
                    doctor.AccountId = null;
                    doctor.Status = null;
                }
            }

            return new(clientDoctors);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientDoctorModel>> GetDoctor(Guid id)
        {
            var dbDoctor = await _dbService.GetDoctor(id);

            var clientDoctor = _mapper.Map<ClientDoctorModel>(dbDoctor);

            if (!User.IsInRole("receptionist") && !CurrentUserIsDoctor(dbDoctor.AccountId))
            {
                clientDoctor.DateOfBirth = null;
                clientDoctor.Email = null;
                clientDoctor.AccountId = null;
                clientDoctor.Status = null;
            }

            return clientDoctor;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize("doctors.edit")]
        public async Task<ActionResult<ClientDoctorModel>> DeleteDoctor(Guid id)
        {
            var dbDoctor = await _dbService.DeleteDoctor(id);

            Log.Information("Doctor deleted => {@dbDoctor}", dbDoctor);

            var clientDoctor = _mapper.Map<ClientDoctorModel>(dbDoctor);

            return clientDoctor;
        }

        [HttpPost]
        [Authorize("doctors.edit")]
        public async Task<ActionResult<ClientDoctorModel>> CreateDoctor([FromBody] ClientDoctorModel doctor)
        {
            var dbDoctor = _mapper.Map<DbDoctorModel>(doctor);

            var addedDoctor = await _dbService.AddDoctor(dbDoctor);

            Log.Information("Doctor created => {@addedDoctor}", addedDoctor);

            var clientDoctor = _mapper.Map<ClientDoctorModel>(addedDoctor);

            return clientDoctor;
        }

        [HttpPut]
        public async Task<ActionResult<ClientDoctorModel>> UpdateDoctor([FromBody] ClientDoctorModel doctor)
        {
            var dbDoctor = _mapper.Map<DbDoctorModel>(doctor);

            if (User.IsInRole("receptionist") || CurrentUserIsDoctor(dbDoctor.AccountId))
            {
                var updatedDoctor = await _dbService.UpdateDoctor(dbDoctor);

                Log.Information("Doctor updated => {@updatedDoctor}", updatedDoctor);

                var clientDoctor = _mapper.Map<ClientDoctorModel>(updatedDoctor);

                return clientDoctor;
            }
            else
            {
                throw new ProfilesException("Forbidden", 403);
            }
        }

        private bool CurrentUserIsDoctor(Guid doctorAccountId)
        {
            return User.IsInRole("doctor") && doctorAccountId.ToString() == User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }
    }
}