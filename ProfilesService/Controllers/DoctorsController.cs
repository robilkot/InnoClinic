using AutoMapper;
using CommonData.Constants;
using CommonData.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Data.Models;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDoctorModel>>> GetDoctors([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] IEnumerable<Guid>? specializations, [FromQuery] IEnumerable<Guid>? offices, [FromQuery] string? doctorName, [FromQuery] IEnumerable<DoctorStatusEnum>? status)
        {
            IEnumerable<ClientDoctorModel> clientDoctors;

            bool userHasAccess = User.IsInRole(Roles.Receptionist);
            
            if (!userHasAccess)
            {
                status = new List<DoctorStatusEnum>() { DoctorStatusEnum.AtWork };
            }
            
            IEnumerable<DbDoctorModel> dbDoctors = await _dbService.GetDoctors(pageNumber, pageSize, specializations, offices, status, doctorName);

            clientDoctors = _mapper.Map<IEnumerable<ClientDoctorModel>>(dbDoctors);
            
            if (!userHasAccess)
            {
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

            if (!User.IsInRole(Roles.Receptionist) && !CurrentUserIsDoctor(dbDoctor.AccountId))
            {
                clientDoctor.DateOfBirth = null;
                clientDoctor.Email = null;
                clientDoctor.AccountId = null;
                clientDoctor.Status = null;
            }

            return clientDoctor;
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteDoctor(Guid id)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            await _dbService.DeleteDoctor(id);

            Log.Information("Doctor deleted: {@record}", id);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ClientDoctorModel>> CreateDoctor([FromBody] ClientDoctorModel doctor)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbDoctor = _mapper.Map<DbDoctorModel>(doctor);

            var addedDoctor = await _dbService.AddDoctor(dbDoctor);

            Log.Information("Doctor created: {@record}", (addedDoctor.Id, addedDoctor.LastName, addedDoctor.OfficeAddress));

            var clientDoctor = _mapper.Map<ClientDoctorModel>(addedDoctor);

            return clientDoctor;
        }

        [HttpPut]
        public async Task<ActionResult<ClientDoctorModel>> UpdateDoctor([FromBody] ClientDoctorModel doctor)
        {
            var dbDoctor = _mapper.Map<DbDoctorModel>(doctor);

            if (!User.IsInRole(Roles.Receptionist) && !CurrentUserIsDoctor(dbDoctor.AccountId))
            {
                return Forbid();
            }

            var updatedDoctor = await _dbService.UpdateDoctor(dbDoctor);

            Log.Information("Doctor updated: {@record}", (updatedDoctor.Id, updatedDoctor.LastName, updatedDoctor.OfficeAddress));

            var clientDoctor = _mapper.Map<ClientDoctorModel>(updatedDoctor);

            return clientDoctor;
        }

        private bool CurrentUserIsDoctor(Guid doctorAccountId)
        {
            return User.IsInRole(Roles.Doctor) && doctorAccountId.ToString() == User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }
    }
}