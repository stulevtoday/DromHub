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
    public sealed partial class SupplierLayoutPage : Page
    {
        public SupplierLayoutPage()
        {
            this.InitializeComponent();
            this.Loaded += SupplierLayoutPage_Loaded;

            // �������� �� ������� ��������� ���������� � ������ ��� ����������� ����������� ������������.
            if (this.DataContext is SupplierLayoutViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ���������� �������� ��������. ��������� ������ ����������� �� ����.
        /// </summary>
        private async void SupplierLayoutPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierLayoutViewModel vm)
            {
                await vm.LoadSupplierLayoutsAsync();
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
        /// ��������� ������� ��������� ����������� � ���� � ��������� ������.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierLayoutViewModel vm)
            {
                await vm.SaveSupplierLayoutsAsync();
                await vm.LoadSupplierLayoutsAsync();
            }
        }
    }
}
