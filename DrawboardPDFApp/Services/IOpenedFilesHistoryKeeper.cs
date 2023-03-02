using DrawboardPDFApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IOpenedFilesHistoryKeeper
    {
        Task<PdfFileInfo> AddRecordAsync(StorageFile file);
        Task<List<PdfFileInfo>> GetAllPdfFilesAsync();
        Task<bool> RecordExistsAsync(StorageFile file);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(StorageFile file);
    }
}