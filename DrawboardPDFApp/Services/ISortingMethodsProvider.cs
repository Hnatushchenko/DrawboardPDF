using DrawboardPDFApp.Models;
using System.Collections.Generic;

namespace DrawboardPDFApp.Services
{
    public interface ISortingMethodsProvider
    {
        IReadOnlyCollection<PdfFileInfoSortingMethod> SortingMethods { get; }
    }
}