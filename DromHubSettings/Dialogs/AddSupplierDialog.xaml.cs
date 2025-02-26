using Microsoft.UI.Xaml.Controls;
using DromHubSettings.Models;
using DromHubSettings.Serviñes;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace DromHubSettings.Dialogs
{
    public sealed partial class AddSupplierDialog : ContentDialog
    {
        public AddSupplierDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddSupplierDialog_Loaded;
        }

        private async void AddSupplierDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var localities = await DataService.LoadLocalitiesAsync();
            LocalityComboBox.ItemsSource = localities;
        }

        public string SupplierName => NameTextBox.Text;
        public string SupplierEmail => EmailTextBox.Text;
        public int SupplierIndex => (int)IndexNumberBox.Value;
        public Guid SelectedLocalityId
        {
            get
            {
                if (LocalityComboBox.SelectedValue is Guid id)
                    return id;
                else
                    return Guid.Empty;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(SupplierName) || LocalityComboBox.SelectedItem == null)
            {
                args.Cancel = true;
            }
        }
    }
}
