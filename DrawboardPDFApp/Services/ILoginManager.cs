using System.ComponentModel;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public interface ILoginManager : INotifyPropertyChanged
    {
        Task LoginSilentlyIfPossibleAsync();
        Task LoginAsync();
        Task LogoutAsync();
        bool IsUserLoggedIn { get; }
    }
}