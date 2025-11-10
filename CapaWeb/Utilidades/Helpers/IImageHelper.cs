namespace CapaWeb.Utilidades.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        //Task<string> UploadImageAsync(IFormFile imageFile);
    }
}
