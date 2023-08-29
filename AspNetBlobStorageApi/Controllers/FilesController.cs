using System.Text;
using Azure.Storage.Blobs;
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
        private readonly BlobContainerClient _blobContainerClient;

        public FilesController(IAzureBlobStorageService fileService, BlobContainerClient blobContainerClient)
        {
            _azureBlobStorageService = fileService;
            _blobContainerClient = blobContainerClient;
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

        [HttpGet("stream")]
        public async Task<IActionResult> GetStreamingAsync([FromQuery] Guid id)
        {
            var memoryStream = new MemoryStream();

            var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
            await blobClient.DownloadToAsync(memoryStream);

            memoryStream.Position = 0;

            return File(memoryStream, _fileType);
        }
    }
}