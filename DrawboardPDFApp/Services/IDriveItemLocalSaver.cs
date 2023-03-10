using Microsoft.Graph;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IDriveItemLocalSaver
    {
        Task<StorageFile> SaveToLocalStorageAsync(DriveItem driveItem);
    }
}