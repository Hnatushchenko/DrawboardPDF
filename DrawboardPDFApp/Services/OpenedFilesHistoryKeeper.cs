using DrawboardPDFApp.Enums;
using DrawboardPDFApp.Extensions;
using DrawboardPDFApp.Models;
using DrawboardPDFApp.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private readonly ISortingMethodsProvider sortingMethodsProvider;

        public OpenedFilesHistoryKeeper(IApplicationContext applicationContext,
            IPdfCoversService pdfCoversService,
            ICloudStorage cloudStorage,
            ISortingMethodsProvider sortingMethodsProvider)
        {
            this.applicationContext = applicationContext;
            this.pdfCoversService = pdfCoversService;
            this.cloudStorage = cloudStorage;
            this.sortingMethodsProvider = sortingMethodsProvider;
            LocalRecords = new NotifyTaskCompletion<ObservableCollection<PdfFileInfo>>(GetPdfFilesHistoryAsObservableAsync());
            CloudRecords.CollectionChanged += RecordsChanged;
        }

        private void RecordsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.Cast<PdfFileInfo>())
                    {
                        AllRecords.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.Cast<PdfFileInfo>())
                    {
                        AllRecords.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.Cast<PdfFileInfo>())
                    {
                        AllRecords.Remove(item);
                    }
                    foreach (var item in e.NewItems.Cast<PdfFileInfo>())
                    {
                        AllRecords.Add(item);
                    }
                    break;
                default:
                    break;
            }
            if (e.Action != NotifyCollectionChangedAction.Remove)
            {
                AllRecords.Sort(sortingMethodsProvider.SelectedMethod.Comparison);
            }
        }

        private NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> LocalRecords { get; set; }
        public ObservableCollection<PdfFileInfo> CloudRecords { get; private set; } = new ObservableCollection<PdfFileInfo>();
        public ObservableCollection<PdfFileInfo> AllRecords { get; private set; } = new ObservableCollection<PdfFileInfo>();

        private async Task RecordLocalFileOpeningAsync(StorageFile file)
        {
            if (RecordExistsLocally(file))
            {
                await UpdateAsync(file, Location.Local);
            }
            else
            {
                await AddLocalRecordAsync(file);
            }
        }

        private async Task RecordCloudFileOpeningAsync(StorageFile file)
        {
            await UpdateAsync(file, Location.Cloud);
        }

        public async Task RecordFileOpeningAsync(StorageFile file, Location location)
        {
            switch (location)
            {
                case Location.Local:
                    await RecordLocalFileOpeningAsync(file);
                    break;
                case Location.Cloud:
                    await RecordCloudFileOpeningAsync(file);
                    break;
            }
        }

        public async Task AddLocalRecordIfNotExistAsync(StorageFile file)
        {
            if (!RecordExistsLocally(file))
            {
                await AddLocalRecordAsync(file);
            }
        }

        public async Task RemoveLocalFileAsync(Guid id)
        {
            var fileToRemove = await applicationContext.OpenedPdfFilesHistory.FirstOrDefaultAsync(x => x.Id == id);
            applicationContext.OpenedPdfFilesHistory.Remove(fileToRemove);
            await applicationContext.SaveChangesAsync();
            var fileInfo = LocalRecords.Result.FirstOrDefault(x => x.Id == id);
            LocalRecords.Result.Remove(fileInfo);
        }

        private bool RecordExistsLocally(StorageFile file)
        {
            var exists = LocalRecords.Result.Any(fileInfo => fileInfo.Path == file.Path);
            return exists;
        }

        private async Task AddLocalRecordAsync(StorageFile file)
        {
            var fileInfo = await CreateRecordAsync(file, Location.Local);
            applicationContext.OpenedPdfFilesHistory.Add(fileInfo);
            await applicationContext.SaveChangesAsync();
            LocalRecords.Result.Add(fileInfo);
        }

        private async Task UpdateAsync(StorageFile file, Location location)
        {
            if (location == Location.Local)
            {
                var fileInfo = await applicationContext.OpenedPdfFilesHistory.FirstAsync(info => info.Path == file.Path);
                fileInfo.LastTimeOpened = DateTimeOffset.Now;
                await applicationContext.SaveChangesAsync();
            }
            else if (location == Location.Cloud)
            {
                var fileInfo = CloudRecords.First(info => info.DisplayName == file.Path);
                fileInfo.LastTimeOpened = DateTimeOffset.Now;
                // TODO: Push updates to the cloud.
            }
        }

        private async Task<PdfFileInfo> CreateRecordAsync(StorageFile file, Location location)
        {
            string coverPath = await pdfCoversService.CreateCoverAsync(file);
            string fileToken = StorageApplicationPermissions.FutureAccessList.Add(file);
            var fileInfo = new PdfFileInfo(coverPath, file.Name, DateTimeOffset.Now, file.DateCreated, file.Path, fileToken, location);
            return fileInfo;
        }

        private async Task<List<PdfFileInfo>> GetAllPdfFilesAsync()
        {
            return await applicationContext.OpenedPdfFilesHistory.AsNoTracking().ToListAsync();
        }

        private async Task<ObservableCollection<PdfFileInfo>> GetPdfFilesHistoryAsObservableAsync()
        {
            var localPdfFiles = await GetAllPdfFilesAsync();
            var observableOpenedPdfFilesHistory = new ObservableCollection<PdfFileInfo>();
            observableOpenedPdfFilesHistory.CollectionChanged += RecordsChanged;
            foreach (var pdfFileInfo in localPdfFiles)
            {
                observableOpenedPdfFilesHistory.Add(pdfFileInfo);
            }
            return observableOpenedPdfFilesHistory;
        }

        public async Task DownloadRecordsFromCloudAsync()
        {
            var pdfFiles = await cloudStorage.GetAllPdfFilesAsync();
            foreach (StorageFile file in pdfFiles)
            {
                await AddCloudRecordIfNotExistAsync(file);
            }
        }

        public void ClearCloudRecords()
        {
            foreach (var cloudRecord in CloudRecords)
            {
                AllRecords.Remove(cloudRecord);
            }
            CloudRecords.Clear();
        }

        private bool RecordExistsInCloud(StorageFile file)
        {
            var exists = CloudRecords.Any(record => record.DisplayName == file.Name);
            return exists;
        }

        public async Task RemoveCloudFileAsync(string name)
        {
            var cloudRecord = CloudRecords.FirstOrDefault(record => record.DisplayName == name);
            CloudRecords.Remove(cloudRecord);
            await cloudStorage.RemoveFileAsync(name);
        }

        public async Task AddCloudRecordIfNotExistAsync(StorageFile file)
        {
            if (RecordExistsInCloud(file))
            {
                return;
            }

            var fileInfo = await CreateRecordAsync(file, Location.Cloud);
            CloudRecords.Add(fileInfo);
            CloudRecords.Sort(sortingMethodsProvider.SelectedMethod.Comparison);
        }
    }
}
