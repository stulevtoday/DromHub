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
    public sealed partial class SupplierExcelSettingsPage : Page
    {
        public SupplierExcelSettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += SupplierExcelSettingsPage_Loaded;

            // �������� �� ������� ��������� ���������� � ������ ��� ����������� ����������� ������������.
            if (this.DataContext is SupplierExcelSettingViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ���������� �������� ��������. ��������� ������ ����������� �� ����.
        /// </summary>
        private async void SupplierExcelSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierExcelSettingViewModel vm)
            {
                await vm.ReadSupplierExcelSettingsAsync();
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
        private async void AddMappingButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddSupplierExcelMappingDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (this.DataContext is SupplierExcelSettingViewModel vm)
                {
                    var newSupplierExcelMapping = new SupplierExcelMapping
                    {
                        SupplierExcelSettingsId = vm.SelectedSupplierExcelSetting.Id,
                        ExcelMappingId = dialog.ExcelMappingId,
                        ColumnIndex = dialog.ColumnIndex
                    };
                    vm.SelectedSupplierExcelSetting.Mappings.Add(newSupplierExcelMapping);
                    await DataService.CreateSupplierExcelMappingAsync(newSupplierExcelMapping);
                }
            }
        }

        /// <summary>
        /// ���������� ������ "��������� ���������".
        /// ��������� ������� ��������� ����������� � ���� � ��������� ������.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierExcelSettingViewModel vm)
            {
                await vm.UpdateSupplierExcelSettingsAsync();
                await vm.ReadSupplierExcelSettingsAsync();
            }
        }
    }
}
