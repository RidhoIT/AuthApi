using AuthApi.Models;

namespace AuthApi.Services
{
    public interface ITokenservice
    {
        string CreateAccessToken(ApplicationUser user, IList<string> roles); 
        RefreshToken CreateRefreshToken(string iPAddress);
    }
}
