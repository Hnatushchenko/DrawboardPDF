using DrawboardPDFApp.Enums;
using DrawboardPDFApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public interface IOpenedFilesHistoryKeeper
    {
        ObservableCollection<PdfFileInfo> CloudRecords { get; }
        ObservableCollection<PdfFileInfo> AllRecords { get; }

        Task AddCloudRecordIfNotExistAsync(StorageFile file);
        Task AddLocalRecordIfNotExistAsync(StorageFile file);
        Task DownloadRecordsFromCloudAsync();
        Task RecordFileOpeningAsync(StorageFile file, Location location);
        Task RemoveAsync(Guid id);
        void ClearCloudRecords();
    }
}