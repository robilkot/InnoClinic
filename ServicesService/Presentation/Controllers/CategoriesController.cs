using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServicesService.Domain.Entities;
using ServicesService.Domain.Interfaces;

namespace ServicesService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryDBService _dbService;
        private readonly IMapper _mapper;
        public CategoriesController(ICategoryDBService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientCategoryModel>>> GetCategories([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            IEnumerable<ClientCategoryModel> clientServices;

            var dbServices = await _dbService.Get(pageNumber, pageSize);

            clientServices = _mapper.Map<IEnumerable<ClientCategoryModel>>(dbServices);

            return new(clientServices);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ClientCategoryModel>> GetService(Guid id)
        {
            var dbService = await _dbService.Get(id);

            var clientService = _mapper.Map<ClientCategoryModel>(dbService);

            return clientService;
        }

        [HttpDelete("{id:Guid}")]
        [Authorize("services.edit")]
        public async Task<ActionResult> DeleteService(Guid id)
        {
            await _dbService.Delete(id);

            Log.Information("Category deleted => {@dbService}", id);

            return NoContent();
        }

        [HttpPost]
        [Authorize("services.edit")]
        public async Task<ActionResult<ClientCategoryModel>> CreateService([FromBody] ClientCategoryModel service)
        {
            var dbPatient = _mapper.Map<Category>(service);

            var addedService = await _dbService.Add(dbPatient);

            Log.Information("Category created => {@addedService}", addedService.Name);

            var clientService = _mapper.Map<ClientCategoryModel>(addedService);

            return clientService;
        }

        [HttpPut]
        [Authorize("services.edit")]
        public async Task<ActionResult<ClientCategoryModel>> UpdateService([FromBody] ClientCategoryModel service)
        {
            var dbPatient = _mapper.Map<Category>(service);

            var updatedService = await _dbService.Update(dbPatient);

            Log.Information("Category updated => {@updatedService}", updatedService.Name);

            var clientService = _mapper.Map<ClientCategoryModel>(updatedService);

            return clientService;
        }
    }
}
