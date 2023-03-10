using DrawboardPDFApp.Enums;
using DrawboardPDFApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IOpenedFilesHistoryKeeper
    {
        NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> Records { get; }
        Task RemoveAsync(Guid id);
        Task RecordFileOpeningAsync(StorageFile file, Location location);
        Task AddRecordIfNotExistAsync(StorageFile file, Location location);
        Task DownloadRecordsFromCloudAsync();
    }
}