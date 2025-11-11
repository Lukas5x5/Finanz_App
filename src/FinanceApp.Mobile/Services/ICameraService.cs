namespace FinanceApp.Mobile.Services;

public interface ICameraService
{
    Task<CameraResult?> TakePhotoAsync();
    Task<CameraResult?> PickPhotoAsync();
}

public class CameraResult
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "image/jpeg";
}
