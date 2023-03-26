using Azure.Storage.Blobs;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Azure Storage Account. 
// Ref: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-client-management?tabs=dotnet#create-a-blobserviceclient-object
// Ref: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=visual-studio%2Cmanaged-identity%2Croles-azure-portal%2Csign-in-azure-cli%2Cidentity-visual-studio
// Ref: https://learn.microsoft.com/pt-br/azure/storage/common/storage-use-azurite?tabs=visual-studio
// Ref: https://hub.docker.com/_/microsoft-azure-storage-azurite
builder.Services.AddSingleton<BlobContainerClient>((_) =>
{
    var serviceClient = new BlobServiceClient(builder.Configuration.GetConnectionString("AZURE_STORAGE_ACCOUNT"));

    var containerName = builder.Configuration.GetSection("ContainerName").Value;
    var container = serviceClient.GetBlobContainerClient(containerName);
    container.CreateIfNotExists();

    return container;
});
builder.Services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
