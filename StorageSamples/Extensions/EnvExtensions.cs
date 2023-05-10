namespace StorageSamples.Extensions;

public static class EnvExtensions {
    public static bool IsQA(this IHostEnvironment hostEnvironment) {
        return hostEnvironment.IsEnvironment("QA");
    }
}

