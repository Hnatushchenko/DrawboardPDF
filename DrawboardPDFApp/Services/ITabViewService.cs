using System;

namespace DrawboardPDFApp.Services
{
    public interface ITabViewService
    {
        void AddTab(string header, Type sourcePageType, object parameter);
    }
}