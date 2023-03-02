using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IPdfCoversService
    {
        Task<string> CreateCoverAsync(StorageFile pdfFile);
    }
}