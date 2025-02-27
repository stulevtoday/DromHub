using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using DromHubSettings.ViewModels;
using DromHubSettings.Models;
using DromHubSettings.Servi�es;
using DromHubSettings.Dialogs;

namespace DromHubSettings.Pages
{
    /// <summary>
    /// �������� ���������� ������������.
    /// ����� ����� �������������, ���������, ������������� � ������� ������ �����������.
    /// </summary>
    public sealed partial class SuppliersPage : Page
    {
        public SuppliersPage()
        {
            this.InitializeComponent();
            this.Loaded += SupplierPage_Loaded;

            // �������� �� ������� ��������� ���������� � ������ ��� ����������� ����������� ������������.
            if (this.DataContext is SupplierViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ���������� �������� ��������. ��������� ������ ����������� �� ����.
        /// </summary>
        private async void SupplierPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierViewModel vm)
            {
                await vm.LoadSuppliersAsync();
            }
        }

        /// <summary>
        /// ���������� ���������� ���� �� �������� ���������� ��������.
        /// </summary>
        private async void Vm_Succeeded(object sender, string content)
        {
            var successDialog = new ContentDialog
            {
                Title = "�������",
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        /// <summary>
        /// ���������� ���������� ���� � ���������� �� ������.
        /// </summary>
        private async void Vm_Fail(object sender, string content)
        {
            var errorDialog = new ContentDialog
            {
                Title = "������",
                Content = "������: " + content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        /// <summary>
        /// ���������� ������ "�������� ����������".
        /// ��������� ���������� ���� ��� ����� ������ ������ ����������, ����� ��������� ��� � ��������� � ��������� � ����.
        /// </summary>
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
                    Index = dialog.SupplierIndex,
                    LocalityId = dialog.SelectedLocalityId
                };

                if (this.DataContext is SupplierViewModel vm)
                {
                    vm.Suppliers.Add(newSupplier);
                    await DataService.AddSupplierAsync(newSupplier);
                }
            }
        }

        /// <summary>
        /// ���������� ������ "��������� ���������".
        /// ��������� ������� ��������� ����������� � ���� � ��������� ������.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierViewModel vm)
            {
                await vm.SaveSuppliersAsync();
                await vm.LoadSuppliersAsync();
            }
        }
    }
}
