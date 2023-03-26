namespace Infrastructure
{
    public interface IAzureBlobStorageService
    {
        Task UploadAsync(Guid id, Stream content);
        Task<Stream?> GetByIdAsync(Guid id);
    }
}