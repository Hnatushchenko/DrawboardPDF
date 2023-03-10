using DrawboardPDFApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Models
{
    public class PdfFileInfo
    {
        public PdfFileInfo(string firstPageImagePath, string displayName, DateTimeOffset lastTimeOpened, DateTimeOffset dateCreated, string path, string fileToken, Location location)
        {
            FirstPageImagePath = firstPageImagePath;
            DisplayName = displayName;
            LastTimeOpened = lastTimeOpened;
            DateCreated = dateCreated;
            Path = path;
            FileToken = fileToken;
            Location = location;
        }

        public Guid Id { get; set; }
        public string FirstPageImagePath { get; set; }
        public string DisplayName { get; set; }
        public DateTimeOffset LastTimeOpened { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string Path { get; set; }
        public string FileToken { get; set; }
        public Location Location { get; set; }
    }
}
