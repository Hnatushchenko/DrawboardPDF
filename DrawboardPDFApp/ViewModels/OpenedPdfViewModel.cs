using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace DrawboardPDFApp.ViewModels
{
    public class OpenedPdfViewModel : ObservableObject
    {
        public async Task CreatePdfPagesFromFileAsync(StorageFile pdfFile)
        {
            PdfPages.Clear();

            PdfDocument pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);

            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                var page = pdfDocument.GetPage(i);
                var image = await ConvertPdfPageToBitmapImageAsync(page);
                PdfPages.Add(image);
            }
        }

        private async Task<BitmapImage> ConvertPdfPageToBitmapImageAsync(PdfPage page)
        {
            var image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);
            }
            return image;
        }

        public ObservableCollection<BitmapImage> PdfPages { get; set; } = new ObservableCollection<BitmapImage>();
    }
}
