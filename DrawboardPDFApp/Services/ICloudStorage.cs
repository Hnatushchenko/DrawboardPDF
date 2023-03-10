using DrawboardPDFApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface ICloudStorage
    {
        Task<IEnumerable<StorageFile>> GetAllPdfFilesAsync();
        Task AddFileAsync(StorageFile file);
    }
}
