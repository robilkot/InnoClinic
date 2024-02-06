using CommonData.Constants;
using DocumentsService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using File = DocumentsService.Domain.Entities.File;

namespace DocumentsService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : Controller
    {
        private readonly IFilesRepository _repository;

        public FilesController(IFilesRepository dbService)
        {
            _repository = dbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles([FromQuery] int pageNumber,
                                                                    [FromQuery] int pageSize,
                                                                    [FromQuery] string? name,
                                                                    [FromQuery] Guid? ownerId)
        {
            if (!CurrentUserIsReceptionist() && !CurrentUserIsDoctor())
            {
                return Forbid();
            }

            if(pageNumber < 0 || pageSize < 0)
            {
                return BadRequest();
            }

            var clientFiles = await _repository.Get(pageNumber, pageSize, name, ownerId);

            return Ok(clientFiles);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<File>> GetFile(Guid id)
        {
            var file = await _repository.Get(id);

            if (file.OwnerId != null)
            {
                if (!CurrentUserIsReceptionist() && !CurrentUserIsDoctor() && !CurrentUserHasId(file.OwnerId.Value))
                {
                    return Forbid();
                }
            }

            return Ok(file);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteService(Guid id)
        {
            if (!CurrentUserIsReceptionist() && !CurrentUserIsDoctor())
            {
                return Forbid();
            }

            await _repository.Delete(id);

            Log.Information("Category deleted: {@dbService}", id);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateFile([FromBody] File file)
        {
            if (!CurrentUserIsReceptionist() && !CurrentUserIsDoctor())
            {
                return Forbid();
            }

            var addedFile = await _repository.Add(file);

            Log.Information("File created: {@addedFile}", addedFile);

            return Created(addedFile.ToString(), null);
        }

        [HttpPut]
        public async Task<ActionResult<Guid>> UpdateFile([FromBody] File file)
        {
            if (!CurrentUserIsReceptionist() && !CurrentUserIsDoctor())
            {
                return Forbid();
            }

            var updatedFile = await _repository.Update(file);

            Log.Information("File updated: {@updatedFile}", updatedFile);

            return Ok(updatedFile);
        }

        [HttpPut("{id:Guid}/rename")]
        public async Task<ActionResult> RenameFile([FromQuery] Guid id,
                                                   [FromQuery] string newName)
        {
            if (!CurrentUserIsReceptionist() && !CurrentUserIsDoctor())
            {
                return Forbid();
            }

            await _repository.Rename(id, newName);

            Log.Information("File renamed: {@updatedFile}", (id, newName));

            return Ok();
        }

        private bool CurrentUserHasId(Guid accountId)
        {
            return accountId.ToString() == User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }
        private bool CurrentUserIsDoctor()
        {
            return User.IsInRole(Roles.Doctor);
        }
        private bool CurrentUserIsReceptionist()
        {
            return User.IsInRole(Roles.Receptionist);
        }
    }
}
