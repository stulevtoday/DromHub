using Microsoft.UI.Xaml.Controls;

namespace DromHubSettings.Dialogs
{
    public sealed partial class AddLocalityDialog : ContentDialog
    {
        public AddLocalityDialog()
        {
            this.InitializeComponent();
        }

        // �������� ��� ��������� ��������� �������� �����������
        public string LocalityName => LocalityNameTextBox.Text;

        // �������� ��� ��������� ����� �������� (� ����)
        public int DeliveryTime => (int)DeliveryTimeNumberBox.Value;

        public string ExportEmail => ExportEmailTextBox.Text;


        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // ����� ����� �������� �������������� ���������, ���� ���������
            // ��������, ��������, ��� LocalityName �� ������
            if (string.IsNullOrWhiteSpace(LocalityName))
            {
                args.Cancel = true; // �������� �������� �������
            }
        }
    }
}
