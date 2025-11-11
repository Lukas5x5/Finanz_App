namespace FinanceApp.Web.Services;

public class StorageService : IStorageService
{
    private readonly Supabase.Client _client;
    private const string BucketName = "invoices";

    public StorageService(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<string?> UploadFileAsync(string fileName, byte[] fileData, string contentType)
    {
        try
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var uniqueFileName = $"{timestamp}_{fileName}";

            await _client.Storage
                .From(BucketName)
                .Upload(fileData, uniqueFileName, new Supabase.Storage.FileOptions
                {
                    ContentType = contentType,
                    Upsert = false
                });

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error uploading file: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetSignedUrlAsync(string objectPath, int expiresInSeconds = 3600)
    {
        try
        {
            var url = await _client.Storage
                .From(BucketName)
                .CreateSignedUrl(objectPath, expiresInSeconds);

            return url;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting signed URL: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(string objectPath)
    {
        try
        {
            await _client.Storage
                .From(BucketName)
                .Remove(new List<string> { objectPath });

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting file: {ex.Message}");
            return false;
        }
    }
}
