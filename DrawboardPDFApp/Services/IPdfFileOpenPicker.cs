using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IPdfFileOpenPicker
    {
        Task<StorageFile> PickSingleFileAsync();
    }
}