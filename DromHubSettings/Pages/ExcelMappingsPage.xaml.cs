using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using DromHubSettings.ViewModels;
using DromHubSettings.Models;
using DromHubSettings.ServiÒes;
using DromHubSettings.Dialogs;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExcelMappingsPage : Page
    {
        public ExcelMappingsPage()
        {
            this.InitializeComponent();
            this.Loaded += ExcelMappingsPage_Loaded;

            if (this.DataContext is ExcelMappingViewModel vm)
            {
                vm.Fail += Vm_Fail;
            }

        }

        private async void Vm_Fail(object sender, string content)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Œ¯Ë·Í‡",
                Content = "Œ¯Ë·Í‡: " + content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async void ExcelMappingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ExcelMappingViewModel vm)
            {
                await vm.ReadExcelMappingsAsync();
            }
        }
    }
}
