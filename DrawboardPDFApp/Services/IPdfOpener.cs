using DrawboardPDFApp.Enums;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IPdfOpener
    {
        Task OpenExistingFileAsync(StorageFile file, Location location);
        Task OpenExistingFileAsync(string fileToken);
        Task OpenNewFileAsync();
    }
}