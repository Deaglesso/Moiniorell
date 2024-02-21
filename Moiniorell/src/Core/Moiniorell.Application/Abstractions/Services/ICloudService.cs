using Microsoft.AspNetCore.Http;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface ICloudService
    {
        Task<string> FileCreateAsync(IFormFile file);
        Task<bool> FileDeleteAsync(string filename);

    }
}
