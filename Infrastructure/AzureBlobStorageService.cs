using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly ILogger<AzureBlobStorageService> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger, BlobContainerClient blobContainerClient)
        {
            _logger = logger;
            _blobContainerClient = blobContainerClient;
        }

        public async Task<Stream?> GetByIdAsync(Guid id)
        {
            var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
            if (!await blobClient.ExistsAsync())
            {
                _logger.LogInformation($"{GetType().Name}: file {id} not found.");

                return null;
            }

            _logger.LogInformation($"{GetType().Name}: downloading file {id}.");

            var result = await blobClient.DownloadStreamingAsync();
            return result.Value.Content;
        }

        public async Task UploadAsync(Guid id, Stream content)
        {
            _logger.LogInformation($"{GetType().Name}: uploading file {id}.");

            var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
            await blobClient.UploadAsync(content: content, overwrite: true);
        }
    }
}