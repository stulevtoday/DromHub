using Microsoft.UI.Xaml.Controls;

namespace DromHubSettings.Dialogs
{
    public sealed partial class AddLocalityDialog : ContentDialog
    {
        public AddLocalityDialog()
        {
            this.InitializeComponent();
        }

        // Свойство для получения введённого названия локальности
        public string LocalityName => LocalityNameTextBox.Text;

        // Свойство для получения срока доставки (в днях)
        public int DeliveryTime => (int)DeliveryTimeNumberBox.Value;

        public string ExportEmail => ExportEmailTextBox.Text;


        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Здесь можно добавить дополнительную валидацию, если требуется
            // Например, проверку, что LocalityName не пустое
            if (string.IsNullOrWhiteSpace(LocalityName))
            {
                args.Cancel = true; // отменяем закрытие диалога
            }
        }
    }
}
