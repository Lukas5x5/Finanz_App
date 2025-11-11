namespace FinanceApp.Mobile.Services;

public class CameraService : ICameraService
{
    public async Task<CameraResult?> TakePhotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                return null;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return null;

            return await ProcessPhotoAsync(photo);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error taking photo: {ex.Message}");
            return null;
        }
    }

    public async Task<CameraResult?> PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null) return null;

            return await ProcessPhotoAsync(photo);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error picking photo: {ex.Message}");
            return null;
        }
    }

    private async Task<CameraResult> ProcessPhotoAsync(FileResult photo)
    {
        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return new CameraResult
        {
            FileName = photo.FileName,
            Data = memoryStream.ToArray(),
            ContentType = photo.ContentType ?? "image/jpeg"
        };
    }
}
