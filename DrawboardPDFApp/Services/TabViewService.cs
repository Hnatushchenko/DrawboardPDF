using DrawboardPDFApp.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DrawboardPDFApp.Services
{
    public class TabViewService : ITabViewService
    {
        private readonly TabView tabView;

        public TabViewService(TabView tabView)
        {
            this.tabView = tabView;
        }

        public void AddTab(string header, Type sourcePageType, object pageParameter)
        {
            var newTab = new TabViewItem
            {
                Header = header,
            };

            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(sourcePageType, pageParameter);
            tabView.TabItems.Add(newTab);
            tabView.SelectedItem = newTab;
        }

        public void AddTabOrSelectIfIsOpened(string header, Type sourcePageType, object pageParameter)
        {
            var existingTab = tabView.TabItems.Cast<TabViewItem>().FirstOrDefault(tab => tab.Header.ToString() == header);
            if (existingTab != null)
            {
                tabView.SelectedItem = existingTab;
            }
            else
            {
                AddTab(header, sourcePageType, pageParameter);
            }
        }
    }
}
