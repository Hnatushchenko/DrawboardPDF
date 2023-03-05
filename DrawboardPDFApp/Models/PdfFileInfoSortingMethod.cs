using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Models
{
    public class PdfFileInfoSortingMethod
    {
        public PdfFileInfoSortingMethod(string displayName, Comparison<PdfFileInfo> comparison)
        {
            DisplayName = displayName;
            Comparison = comparison;
        }

        public string DisplayName { get; }
        public Comparison<PdfFileInfo> Comparison { get; }
    }
}
