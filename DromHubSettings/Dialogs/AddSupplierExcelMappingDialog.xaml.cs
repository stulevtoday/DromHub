using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DromHubSettings.Servi�es;

namespace DromHubSettings.Dialogs
{
    public sealed partial class AddSupplierExcelMappingDialog : ContentDialog
    {
        public AddSupplierExcelMappingDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddSupplierExcelMappingDialog_Loaded;
        }

        private async void AddSupplierExcelMappingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var excelMappings = await DataService.ReadExcelMappingsAsync();
            ExcelMappingComboBox.ItemsSource = excelMappings;
        }

        // ���������� ������� ������ "���������" (PrimaryButton)
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // ����� ����� ��������� �������������� ���������.
            // ���� ������ �����������, ����� �������� �������� �������:
            // args.Cancel = true;
        }

        // ���������� ������� ������ "������" (SecondaryButton)
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // ������ �������������� ������ �� ���������.
        }

        /// <summary>
        /// ���������� ��������� �������� �� ComboBox ��� ������������� ���� �������������.
        /// </summary>
        public Guid ExcelMappingId
        {
            get
            {
                if (ExcelMappingComboBox.SelectedValue is Guid id)
                    return id;
                else
                    return Guid.Empty;
            }
        }

        /// <summary>
        /// ���������� �������� ����� �������.
        /// </summary>
        public int ColumnIndex => (int)ColumnIndexNumberBox.Value;
    }
}
