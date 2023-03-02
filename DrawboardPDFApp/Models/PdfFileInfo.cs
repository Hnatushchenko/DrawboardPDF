using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Models
{
    public class PdfFileInfo
    {
        public PdfFileInfo(string firstPageImagePath, string displayName, DateTimeOffset lastTimeOpened, string path)
        {
            FirstPageImagePath = firstPageImagePath;
            DisplayName = displayName;
            LastTimeOpened = lastTimeOpened;
            Path = path;
        }

        public Guid Id { get; set; }
        public string FirstPageImagePath { get; set; }
        public string DisplayName { get; set; }
        public DateTimeOffset LastTimeOpened { get; set; }
        public string Path { get; set; }
    }
}
