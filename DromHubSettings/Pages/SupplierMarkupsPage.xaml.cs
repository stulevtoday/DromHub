using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DromHubSettings.Models;
using DromHubSettings.Servi�es;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace DromHubSettings.Pages
{
    public sealed partial class SupplierMarkupsPage : Page
    {
        public SupplierMarkupsViewModel ViewModel { get; } = new SupplierMarkupsViewModel();

        public SupplierMarkupsPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadSupplierMarkupsAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // �����������, ��� � ��� ���� DataService ��� ������ � ����� ������.
                // ��������, DataService.UpdateBrandMarkupsAsync ��������� ��������� ���������� ������.
                await DataService.SaveSupplierMarkupsAsync(ViewModel.SupplierMarkups);

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

    public class SupplierMarkupsViewModel
    {
        public ObservableCollection<SupplierMarkup> SupplierMarkups { get; } = new ObservableCollection<SupplierMarkup>();

        public async Task LoadSupplierMarkupsAsync()
        {
            var list = await DataService.GetSupplierMarkupsAsync();
            SupplierMarkups.Clear();
            foreach (var sm in list)
            {
                SupplierMarkups.Add(sm);
            }
        }
    }
}
