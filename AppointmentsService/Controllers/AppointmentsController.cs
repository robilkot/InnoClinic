﻿using AppointmentsService.Commands;
using AppointmentsService.Data.Models;
using AppointmentsService.Models;
using AppointmentsService.Queries;
using AutoMapper;
using CommonData.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace AppointmentsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public AppointmentsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientAppointment>>> GetAppointments([FromQuery] int pageNumber, 
                                                                                        [FromQuery] int pageSize,
                                                                                        [FromQuery] DateTime? date, 
                                                                                        [FromQuery] Guid? doctorId, 
                                                                                        [FromQuery] Guid? serviceId, 
                                                                                        [FromQuery] bool? approved, 
                                                                                        [FromQuery] Guid? officeId, 
                                                                                        [FromQuery] Guid? patientId)
        {
            if (User.IsInRole(Roles.Patient))
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id");

                if (userId == null || userId.Value.IsNullOrEmpty())
                {
                    return Forbid();
                }
                else
                {
                    patientId = Guid.Parse(userId.Value);
                }
            }

            var query = new GetAppointmentsQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Date = date,
                DoctorId = doctorId,
                ServiceId = serviceId,
                Approved = approved,
                OfficeId = officeId,
                PatientId = patientId
            };

            var appointments = await _mediator.Send(query);

            var clientAppointments = _mapper.Map<IEnumerable<ClientAppointment>>(appointments);

            return Ok(clientAppointments);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientAppointment>> GetAppointment(Guid id)
        {
            var query = new GetAppointmentByIdQuery()
            {
                Id = id
            };

            DbAppointment appointments = await _mediator.Send(query);

            if (User.IsInRole(Roles.Patient) && !UserIdMatches(appointments.PatientId))
            {
                return Forbid();
            }

            var clientAppointment = _mapper.Map<ClientAppointment>(appointments);

            return new(clientAppointment);
        }

        [HttpPost]
        public async Task<ActionResult> AddAppointment([FromBody] ClientAppointment appointment)
        {
            var dbAppointment = _mapper.Map<DbAppointment>(appointment);

            // todo: how to return guid or object?
            await _mediator.Send(new AddAppointmentCommand() { Appointment = dbAppointment });

            Log.Information("Appointment created: {@record}", (dbAppointment.Id, dbAppointment.ServiceName, dbAppointment.OfficeAddress));

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<ClientAppointment>> UpdateAppointment([FromBody] ClientAppointment clientAppointment)
        {
            if (!UserIdMatches(clientAppointment.PatientId) && !User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var dbAppointment = _mapper.Map<DbAppointment>(clientAppointment);

            var command = new UpdateAppointmentCommand()
            {
                Appointment = dbAppointment
            };

            var updated = await _mediator.Send(command);

            var clientUpdated = _mapper.Map<ClientAppointment>(updated);

            return new(clientUpdated);
        }

        [HttpPut("approve/{id:Guid}")]
        public async Task<ActionResult<ClientAppointment>> ChangeAppointmentApproval(Guid id, [FromQuery] bool approved)
        {
            if (!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var command = new ChangeAppointmentApprovalCommand()
            {
                Id = id,
                IsApproved = approved
            };

            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteAppointment(Guid id)
        {
            if(!User.IsInRole(Roles.Receptionist))
            {
                return Forbid();
            }

            var command = new DeleteAppointmentCommand()
            {
                Id = id
            };

            await _mediator.Send(command);

            Log.Information("Appointment deleted: {@record}", id);

            return NoContent();
        }

        private bool UserIdMatches(Guid? id)
        {
            if (id == null)
            {
                return false;
            }
            return id.ToString() == User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }
    }
}
