using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.IO;

namespace DrawboardPDFApp.Services
{
    public class PdfCoversService : IPdfCoversService
    {
        public async Task<string> CreateCoverAsync(StorageFile pdfFile)
        {
            PdfPage coverPage = await GetFirstPage(pdfFile);
            SoftwareBitmap softwareBitmap = await CreateSoftwareBitmapFromCoverPage(coverPage);
            var coverName = pdfFile.DisplayName + ".jpg";
            var coverPath = await SaveSoftwareBitmapAsJpgInCoversFolder(coverName, softwareBitmap);
            return coverPath;
        }

        private async Task<SoftwareBitmap> CreateSoftwareBitmapFromCoverPage(PdfPage coverPage)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                await coverPage.RenderToStreamAsync(stream);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                return softwareBitmap;
            }
        }

        private async Task<string> SaveSoftwareBitmapAsJpgInCoversFolder(string coverName, SoftwareBitmap softwareBitmap)
        {
            var coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
            StorageFile coverFile = await coversFolder.CreateFileAsync(coverName, CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream outputStream = await coverFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, outputStream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();
            }

            var coverPath = Path.Combine(coversFolder.Path, coverName);
            return coverPath;
        }

        private async Task<PdfPage> GetFirstPage(StorageFile pdfFile)
        {
            var pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);
            PdfPage coverPage = pdfDocument.GetPage(0);
            return coverPage;
        }
    }
}
