using Microsoft.UI.Xaml.Controls;

namespace DromHubSettings.Dialogs
{
    public sealed partial class AddExportColumnDialog : ContentDialog
    {
        public AddExportColumnDialog()
        {
            this.InitializeComponent();
        }

        public string ColumnName => ColumnNameTextBox.Text;

        public int ColumnIndex
        {
            get
            {
                if (int.TryParse(ColumnIndexTextBox.Text, out var val))
                    return val;
                return 1;
            }
        }

        public string MappingProperty => MappingPropertyTextBox.Text;

        public bool IsVisible => IsVisibleCheckBox.IsChecked == true;
    }
}
