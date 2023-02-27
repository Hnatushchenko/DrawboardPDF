using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public interface INavigationService
    {
        void Navigate(Type pageType);
        void Navigate(Type pageType, object parameter);
        void GoBack();
        bool CanGoBack { get; }
    }
}
