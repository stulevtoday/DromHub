using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DromHubSettings.Serviсes;

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

        // Обработчик нажатия кнопки "Сохранить" (PrimaryButton)
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Здесь можно выполнить дополнительную валидацию.
            // Если данные некорректны, можно отменить закрытие диалога:
            // args.Cancel = true;
        }

        // Обработчик нажатия кнопки "Отмена" (SecondaryButton)
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Обычно дополнительная логика не требуется.
        }

        /// <summary>
        /// Возвращает выбранное значение из ComboBox как идентификатор поля сопоставления.
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
        /// Возвращает введённый номер столбца.
        /// </summary>
        public int ColumnIndex => (int)ColumnIndexNumberBox.Value;
    }
}
