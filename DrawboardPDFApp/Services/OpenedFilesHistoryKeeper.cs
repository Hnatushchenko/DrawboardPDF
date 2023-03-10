using DrawboardPDFApp.Enums;
using DrawboardPDFApp.Models;
using DrawboardPDFApp.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace DrawboardPDFApp.Services
{
    public class OpenedFilesHistoryKeeper : IOpenedFilesHistoryKeeper
    {
        private readonly IApplicationContext applicationContext;
        private readonly IPdfCoversService pdfCoversService;
        private readonly ICloudStorage cloudStorage;

        public OpenedFilesHistoryKeeper(IApplicationContext applicationContext,
            IPdfCoversService pdfCoversService,
            ICloudStorage cloudStorage)
        {
            this.applicationContext = applicationContext;
            this.pdfCoversService = pdfCoversService;
            this.cloudStorage = cloudStorage;
            Records = new NotifyTaskCompletion<ObservableCollection<PdfFileInfo>>(GetPdfFilesHistoryAsObservableAsync());
        }

        public NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> Records { get; private set; }

        public async Task RecordFileOpeningAsync(StorageFile file, Location location)
        {
            if (RecordExists(file))
            {
                await UpdateAsync(file);
            }
            else
            {
                await AddRecordAsync(file, location);
            }
        }

        public async Task AddRecordIfNotExistAsync(StorageFile file, Location location)
        {
            if (!RecordExists(file))
            {
                await AddRecordAsync(file, location);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            var fileToRemove = await applicationContext.OpenedPdfFilesHistory.FirstOrDefaultAsync(x => x.Id == id);
            applicationContext.OpenedPdfFilesHistory.Remove(fileToRemove);
            await applicationContext.SaveChangesAsync();
            var fileInfo = Records.Result.FirstOrDefault(x => x.Id == id);
            Records.Result.Remove(fileInfo);
        }

        private bool RecordExists(StorageFile file)
        {
            return Records.Result.Any(fileInfo => fileInfo.Path == file.Path);
        }

        private async Task AddRecordAsync(StorageFile file, Location location)
        {
            string coverPath = await pdfCoversService.CreateCoverAsync(file);
            string fileToken = StorageApplicationPermissions.FutureAccessList.Add(file);
            var fileInfo = new PdfFileInfo(coverPath, file.Name, DateTimeOffset.Now, file.DateCreated, file.Path, fileToken, location);
            applicationContext.OpenedPdfFilesHistory.Add(fileInfo);
            await applicationContext.SaveChangesAsync();
            Records.Result.Add(fileInfo);
        }

        private async Task UpdateAsync(StorageFile file)
        {
            var fileInfo = await applicationContext.OpenedPdfFilesHistory.FirstAsync(info => info.Path == file.Path);
            fileInfo.LastTimeOpened = DateTimeOffset.Now;
            await applicationContext.SaveChangesAsync();
            // TODO: Update on UI as well;
        }

        private async Task<List<PdfFileInfo>> GetAllPdfFilesAsync()
        {
            return await applicationContext.OpenedPdfFilesHistory.AsNoTracking().ToListAsync();
        }

        private async Task<ObservableCollection<PdfFileInfo>> GetPdfFilesHistoryAsObservableAsync()
        {
            var openedPdfFilesHistory = await GetAllPdfFilesAsync();
            var observableOpenedPdfFilesHistory = new ObservableCollection<PdfFileInfo>(openedPdfFilesHistory);
            return observableOpenedPdfFilesHistory;
        }

        public async Task DownloadRecordsFromCloudAsync()
        {
            var pdfFiles = await cloudStorage.GetAllPdfFilesAsync();
            foreach (StorageFile file in pdfFiles)
            {
                await AddRecordIfNotExistAsync(file, Location.Cloud);
            }
        }
    }
}
