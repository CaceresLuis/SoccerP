using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface IImageHelper
    {
        string UploadImage(byte[] pictureArray, string folder);
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
