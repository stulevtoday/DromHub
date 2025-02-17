using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;
using DromHubSettings.ViewModels;

namespace DromHubSettings.Pages
{
    public sealed partial class ExportLayoutPage : Page
    {
        public ExportLayoutViewModel ViewModel { get; } = new ExportLayoutViewModel();

        public ExportLayoutPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadAsync();
        }

        private void AddMappingButton_Click(object sender, RoutedEventArgs e)
        {
            var newMapping = new ExportLayoutMapping
            {
                Id = Guid.NewGuid(),
                FieldKey = "", // пользователь введёт значение, например, "catalog"
                HeaderText = "", // пользователь вводит заголовок
                ColumnNumber = ViewModel.Mappings.Any() ? ViewModel.Mappings.Max(m => m.ColumnNumber) + 1 : 1
            };
            ViewModel.Mappings.Add(newMapping);
        }

        private async void SaveMappingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ViewModel.SaveAsync();
                var successDialog = new ContentDialog
                {
                    Title = "Сохранено",
                    Content = "Разметка успешно обновлена.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Ошибка при сохранении",
                    Content = "Произошла ошибка: " + ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }

        private void DeleteMappingButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ExportLayoutMapping mapping)
            {
                ViewModel.Mappings.Remove(mapping);
                // Если требуется, можно вызвать метод удаления из БД.
            }
        }
    }
}
