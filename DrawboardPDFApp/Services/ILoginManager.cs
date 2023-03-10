using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public interface ILoginManager
    {
        Task LoginAsync();
        Task LogoutAsync();
    }
}