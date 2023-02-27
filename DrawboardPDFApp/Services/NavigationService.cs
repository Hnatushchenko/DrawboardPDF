using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DrawboardPDFApp.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Frame frame;

        public NavigationService(Frame frame)
        {
            this.frame = frame;
        }

        public void Navigate(Type pageType)
        {
            frame.Navigate(pageType);
        }

        public void Navigate(Type pageType, object parameter)
        {
            frame.Navigate(pageType, parameter);
        }

        public void GoBack()
        {
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        public bool CanGoBack => frame.CanGoBack;
    }
}
