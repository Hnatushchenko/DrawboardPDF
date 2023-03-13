using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public interface IAuthenticationResultProvider
    {
        Task<AuthenticationResult> GetAuthenticationResultAsync();
        Task<bool> CanAuthenticateSilentlyAsync();
    }
}