using DrawboardPDFApp.Models;
using DrawboardPDFApp.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
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

        public OpenedFilesHistoryKeeper(IApplicationContext applicationContext, IPdfCoversService pdfCoversService)
        {
            this.applicationContext = applicationContext;
            this.pdfCoversService = pdfCoversService;
        }

        public async Task<bool> RecordExistsAsync(StorageFile file)
        {
            return await applicationContext.OpenedPdfFilesHistory.AnyAsync(fileInfo => fileInfo.Path == file.Path);
        }

        public async Task<PdfFileInfo> AddRecordAsync(StorageFile file)
        {
            string coverPath = await pdfCoversService.CreateCoverAsync(file);
            var fileInfo = new PdfFileInfo(coverPath, file.Name, DateTimeOffset.Now, file.Path);
            applicationContext.OpenedPdfFilesHistory.Add(fileInfo);
            await applicationContext.SaveChangesAsync();
            return fileInfo;
        }

        public async Task UpdateAsync(StorageFile file)
        {
            var fileInfo = await applicationContext.OpenedPdfFilesHistory.FirstAsync(info => info.Path == file.Path);
            fileInfo.LastTimeOpened = DateTimeOffset.Now;
            await applicationContext.SaveChangesAsync();
            // TODO: Update on UI as well;
        }

        public async Task<List<PdfFileInfo>> GetAllPdfFilesAsync()
        {
            return await applicationContext.OpenedPdfFilesHistory.AsNoTracking().ToListAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var fileToRemove = await applicationContext.OpenedPdfFilesHistory.FirstOrDefaultAsync(x => x.Id == id);
            applicationContext.OpenedPdfFilesHistory.Remove(fileToRemove);
            await applicationContext.SaveChangesAsync();
        }
    }
}
