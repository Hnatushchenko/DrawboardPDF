using DrawboardPDFApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace DrawboardPDFApp.Services
{
    public class SortingMethodsProvider : ISortingMethodsProvider
    {
        private readonly ResourceLoader resourceLoader;

        public SortingMethodsProvider()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();
            CreateSortingMethods();
        }

        public IReadOnlyCollection<PdfFileInfoSortingMethod> SortingMethods { get; private set; }

        private void CreateSortingMethods()
        {
            var sortingMethods = new List<PdfFileInfoSortingMethod>()
            {
                new PdfFileInfoSortingMethod
                (
                    resourceLoader.GetString("Last opened by me"),
                    (f1, f2) => f2.LastTimeOpened.CompareTo(f1.LastTimeOpened)
                ),
                new PdfFileInfoSortingMethod
                (
                    resourceLoader.GetString("Name (A to Z)"),
                    (f1, f2) => f1.DisplayName.CompareTo(f2.DisplayName)
                ),
                new PdfFileInfoSortingMethod
                (
                    resourceLoader.GetString("Name (Z to A)"),
                    (f1, f2) => f2.DisplayName.CompareTo(f1.DisplayName)
                )
            }; 

            SortingMethods = sortingMethods.AsReadOnly();
        }
    }
}
