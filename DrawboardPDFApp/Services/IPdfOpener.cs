using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IPdfOpener
    {
        Task OpenExistingFileAsync(StorageFile file);
        Task OpenExistingFileAsync(string fileToken);
        Task OpenNewFileAsync();
    }
}