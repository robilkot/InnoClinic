using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficesService.Models;
using OfficesService.Services;
using AutoMapper;

namespace OfficesService.Controllers
{
    [ApiController]
    [Authorize(Roles = "receptionist,admin")]
    [Route("[controller]")]
    public class OfficesController : ControllerBase
    {
        private readonly DbService _dbService;
        private readonly IMapper _mapper;
        public OfficesController(DbService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ClientOfficeModel>>> GetOffices()
        {
            var dbOffices = await _dbService.GetOffices();

            var clientOffices = _mapper.Map<IEnumerable<ClientOfficeModel>>(dbOffices);

            return new(clientOffices);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientOfficeModel>> GetOffice(Guid id)
        {
            var dbOffice = await _dbService.GetOffice(id);

            var clientOffice = _mapper.Map<ClientOfficeModel>(dbOffice);

            return clientOffice;
        }

        [HttpPost]
        public async Task<ActionResult<ClientOfficeModel>> CreateOffice([FromBody] ClientOfficeModel office)
        {
            var dbOffice = _mapper.Map<DbOfficeModel>(office);

            var addedOffice = await _dbService.AddOffice(dbOffice);

            var clientOffice = _mapper.Map<ClientOfficeModel>(addedOffice);

            return clientOffice;
        }

        [HttpPut]
        public async Task<ActionResult<ClientOfficeModel>> UpdateOffice([FromBody] ClientOfficeModel office)
        {
            var dbOffice = _mapper.Map<DbOfficeModel>(office);

            var updatedOffice = await _dbService.UpdateOffice(dbOffice);

            var clientOffice = _mapper.Map<ClientOfficeModel>(updatedOffice);

            return clientOffice;
        }
    }
}