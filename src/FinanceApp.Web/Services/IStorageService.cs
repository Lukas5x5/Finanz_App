namespace FinanceApp.Web.Services;

public interface IStorageService
{
    Task<string?> UploadFileAsync(string fileName, byte[] fileData, string contentType);
    Task<string?> GetSignedUrlAsync(string objectPath, int expiresInSeconds = 3600);
    Task<bool> DeleteFileAsync(string objectPath);
}
