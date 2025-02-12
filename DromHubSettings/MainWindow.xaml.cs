using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            MainNavigationView.SelectionChanged += MainNavigationView_SelectionChanged;
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Пример: получение выбранного элемента
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string pageTag = selectedItem.Tag.ToString();
                // Навигация по фрейму
                // При условии, что pageTag совпадает с именем класса страницы, например "MainPage"
                Type pageType = Type.GetType(@"DromHubSettings.Pages." + pageTag);
                if (pageType != null)
                {
                    contentFrame.Navigate(pageType);
                }
            }
        }
    }
}
