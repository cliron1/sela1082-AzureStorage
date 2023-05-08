using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace StorageSamples.Servcies;

public interface IStorageService {
    Task<List<string>> GetFiles();
    Task Upload(IFormFile file);
}

public class AzureStorageService : IStorageService {
    private BlobContainerClient container;

    public AzureStorageService(IConfiguration config) {
        var client = new BlobServiceClient(config.GetConnectionString("Storage"));
        container = client.GetBlobContainerClient("rscs");
        container.CreateIfNotExists();
    }

    public async Task<List<string>> GetFiles() {
        var ret = new List<string>();
        await foreach(BlobItem blobItem in container.GetBlobsAsync()) {
            var blob = container.GetBlobClient(blobItem.Name);
            ret.Add(blob.Uri.ToString());
        }
        return ret;
    }

    public async Task Upload(IFormFile file) {
        var ext = Path.GetExtension(file.FileName).ToLower();
        var fileName = Guid.NewGuid().ToString("N") + ext;
        BlobClient blobClient = container.GetBlobClient(fileName);
        await blobClient.UploadAsync(file.OpenReadStream(), true);

        blobClient.SetHttpHeaders(new BlobHttpHeaders {
            ContentType = file.ContentType
        });
    }
}

public class FileSystemService : IStorageService {
    private readonly string root;
    private readonly string rscs;

    public FileSystemService(IWebHostEnvironment env) {
        root = env.WebRootPath;
        rscs = Path.Combine(root, "rscs");
    }

    public async Task<List<string>> GetFiles() {
        var files = Directory.GetFiles(rscs)
            .Select(path => path.PathToUrl(root))
            .ToList();
        return await Task.FromResult(files);
    }

    public async Task Upload(IFormFile file) {
        var ext = Path.GetExtension(file.FileName).ToLower();
        var fileName = Guid.NewGuid().ToString("N") + ext;
        var filePath = Path.Combine(rscs, fileName);
        using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
        await file.CopyToAsync(fs);
    }
}
