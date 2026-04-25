using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file, string folder);
        Task<bool> DeleteAsync(string fileUrl);
    }
}
