using Infrastructure;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetBlobStorageApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private const string _fileType = "text/csv";
        private readonly IAzureBlobStorageService _azureBlobStorageService;

        public FilesController(IAzureBlobStorageService fileService)
        {
            _azureBlobStorageService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            if (file.ContentType != _fileType)
                return BadRequest("Only csv files can be uploaded.");

            var fileId = Guid.NewGuid();

            await _azureBlobStorageService.UploadAsync(fileId, file.OpenReadStream());

            return Created(new Uri($"{Request.GetEncodedUrl()}/{fileId}"), null);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] Guid id)
        {
            var fileStream = await _azureBlobStorageService.GetByIdAsync(id);
            if (fileStream is null)
                return NotFound();

            return File(fileStream, _fileType);
        }
    }
}