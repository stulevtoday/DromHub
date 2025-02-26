using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DromHubSettings.Models;
using DromHubSettings.Servi�es;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using DromHubSettings.Dialogs;
using DromHubSettings.ViewModels;

namespace DromHubSettings.Pages
{
    public sealed partial class SupplierMarkupsPage : Page
    {
        public SupplierMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += SupplierMarkupsPage_Loaded;

            // �������� �� ������� ��������� ���������� � ������ ��� ����������� ����������� ������������.
            if (this.DataContext is SupplierViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ���������� �������� ��������. ��������� ������ ������� ����������� �� ����.
        /// </summary>
        private async void SupplierMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierMarkupViewModel vm)
            {
                await vm.LoadSupplierMarkupsAsync();
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
        /// ���������� ������ "��������� ���������".
        /// ��������� ������� ��������� ������� ����������� � ���� � ��������� ������.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierMarkupViewModel vm)
            {
                await vm.SaveSupplierMarkupsAsync();
                await vm.LoadSupplierMarkupsAsync();
            }
        }
    }
}
