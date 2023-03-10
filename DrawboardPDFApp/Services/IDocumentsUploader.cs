using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public interface IDocumentsUploader
    {
        Task UploadNewDocumentAsync();
    }
}