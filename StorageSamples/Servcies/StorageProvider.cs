namespace StorageSamples.Servcies;

public interface IStorageProvider {
    IStorageService Service { get; }
}

public class StorageProvider : IStorageProvider {
    public IStorageService Service { get; private set; }

    public StorageProvider(IConfiguration config, IWebHostEnvironment env) {
        var type = config.GetValue<StorageTypes>("Storage:Type");
        
        switch (type) {
            case StorageTypes.FileSystem:
                Service = new FileSystemService(env);
                break;
            case StorageTypes.AzureStorage:
                Service = new AzureStorageService(config);
                break;
            default:
                throw new NotSupportedException();
        }

        Service = type switch {
            StorageTypes.FileSystem => new FileSystemService(env),
            StorageTypes.AzureStorage => new AzureStorageService(config),
            _ => throw new NotImplementedException()
        };

        //Service = type switch {
        //    StorageTypes.FileSystem => new FileSystemService(env),
        //    StorageTypes.AzureStorage => new AzureStorageService(config),
        //    _ => throw new NotSupportedException()
        //};
    }
}

public enum StorageTypes {
    FileSystem = 0,
    AzureStorage = 1
}
