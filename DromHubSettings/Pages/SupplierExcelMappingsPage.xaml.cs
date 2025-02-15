using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using DromHubSettings.Servi�es;
using Microsoft.UI.Xaml;
using DromHubSettings.Models;
using System.Collections.ObjectModel;
using System;

namespace DromHubSettings.Pages
{
    public sealed partial class SupplierExcelMappingsPage : Page
    {
        public SupplierExcelMappingsViewModel ViewModel { get; } = new SupplierExcelMappingsViewModel();

        public SupplierExcelMappingsPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadSupplierExcelMappingsAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // �����������, ��� � ��� ���� DataService ��� ������ � ����� ������.
                // ��������, DataService.UpdateBrandMarkupsAsync ��������� ��������� ���������� ������.
                await DataService.SaveSupplierExcelMappingsAsync(ViewModel.SupplierExcelMappings);

                var successDialog = new ContentDialog
                {
                    Title = "���������",
                    Content = "������ ������� ���������.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "������ ��� ����������",
                    Content = "��������� ������: " + ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }

    public class SupplierExcelMappingsViewModel
    {
        public ObservableCollection<SupplierExcelMapping> SupplierExcelMappings { get; } = new ObservableCollection<SupplierExcelMapping>();

        public async Task LoadSupplierExcelMappingsAsync()
        {
            var mappings = await DataService.GetSupplierExcelMappingsAsync();
            SupplierExcelMappings.Clear();
            foreach (var mapping in mappings)
            {
                SupplierExcelMappings.Add(mapping);
            }
        }
    }
}
