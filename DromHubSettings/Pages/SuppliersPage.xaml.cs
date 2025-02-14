using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using DromHubSettings.Models;
using DromHubSettings.Servi�es;
using System.Threading.Tasks;
using DromHubSettings.Dialogs;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SuppliersPage : Page
    {
        public SuppliersViewModel ViewModel { get; } = new SuppliersViewModel();
        public SuppliersPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadSuppliersAsync();
        }

        // ���� ���������, ����� �������� ����� ��� �������� ����������� �� ��.
        public async Task LoadSuppliersAsync()
        {
            var suppliers = await DataService.GetSuppliersAsync();
            ViewModel.Suppliers.Clear();
            foreach (var supplier in suppliers.OrderBy(b => b.Index))
            {
                ViewModel.Suppliers.Add(supplier);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddSupplierDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newSupplier = new Supplier
                {
                    Id = Guid.NewGuid(),
                    Name = dialog.SupplierName,
                    Email = dialog.SupplierEmail,
                    LocalityName = dialog.SupplierLocality,
                    Index = ViewModel.Suppliers.Count
                };

                await DataService.AddSupplierAsync(newSupplier);
                ViewModel.Suppliers.Add(newSupplier);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Supplier supplier)
            {
                ViewModel.Suppliers.Remove(supplier);
                await DataService.DeleteSupplierAsync(supplier);
            }
        }

        // ���������� ������ � ����
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // �����������, ��� � ��� ���� DataService ��� ������ � ����� ������.
                // ��������, DataService.UpdateBrandMarkupsAsync ��������� ��������� ���������� ������.
                await DataService.SaveSuppliersAsync(ViewModel.Suppliers);

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

    public class SuppliersViewModel
    {
        public ObservableCollection<Supplier> Suppliers { get; } = new ObservableCollection<Supplier>();
    }
}
