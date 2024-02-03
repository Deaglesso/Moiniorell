using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<List<string>> Register(RegisterVM vm);
        Task<List<string>> Login(LoginVM vm);
        Task Logout();
        Task<AppUser> GetUser(string username);

    }

}
