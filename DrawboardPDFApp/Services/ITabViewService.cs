using System;

namespace DrawboardPDFApp.Services
{
    public interface ITabViewService
    {
        void AddTab(string header, Type sourcePageType, object parameter);
        void AddTabOrSelectIfIsOpened(string header, Type sourcePageType, object pageParameter);
    }
}