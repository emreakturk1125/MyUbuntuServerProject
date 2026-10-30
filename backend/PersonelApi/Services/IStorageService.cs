using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PersonelApi.Services
{
    public interface IStorageService
    {
        Task EnsureBucketExistsAsync();
        Task<string> UploadFileAsync(IFormFile file, string folder = "");
        Task<Stream> GetFileAsync(string fileName);
        Task<bool> DeleteFileAsync(string fileName);
    }
}

