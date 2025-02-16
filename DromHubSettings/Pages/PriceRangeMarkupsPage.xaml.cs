using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DromHubSettings.Models;
using DromHubSettings.Dialogs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DromHubSettings.Serviсes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PriceRangeMarkupsPage : Page
    {
        public PriceRangeMarkupsViewModel ViewModel { get; } = new PriceRangeMarkupsViewModel();

        public PriceRangeMarkupsPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadPriceRangeMarkupsAsync();
        }

        private async Task LoadPriceRangeMarkupsAsync()
        {
            ViewModel.PriceRangeMarkups.Clear();
            var list = await DataService.GetPriceRangeMarkupsAsync();
            // Сортировка по минимальной цене
            foreach (var item in list.OrderBy(r => r.MinPrice))
            {
                ViewModel.PriceRangeMarkups.Add(item);
            }
        }

        // Обработчик удаления диапазона
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is PriceRangeMarkup rangeMarkup)
            {
                ViewModel.PriceRangeMarkups.Remove(rangeMarkup);
                await DataService.DeletePriceRangeMarkupAsync(rangeMarkup);
            }
        }

        // Обработчик добавления нового диапазона (открытие диалога для ввода)
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPriceRangeDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newRange = new PriceRangeMarkup
                {
                    Id = Guid.NewGuid(), // генерируем уникальный идентификатор в программе
                    MinPrice = dialog.MinPrice,
                    MaxPrice = dialog.MaxPrice,
                    Markup = dialog.Markup
                };
                ViewModel.PriceRangeMarkups.Add(newRange);
                await DataService.AddPriceRangeMarkupAsync(newRange);
                await LoadPriceRangeMarkupsAsync();
            }
        }

        // Обновление записи в базе при изменении любого из значений диапазона
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Предположим, что у вас есть DataService для работы с базой данных.
                // Например, DataService.UpdateBrandMarkupsAsync принимает коллекцию обновлённых данных.
                await DataService.SavePriceRangeMarkupsAsync(ViewModel.PriceRangeMarkups);

                var successDialog = new ContentDialog
                {
                    Title = "Сохранено",
                    Content = "Данные успешно обновлены.",
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
    }

    public class PriceRangeMarkupsViewModel
    {
        public ObservableCollection<PriceRangeMarkup> PriceRangeMarkups { get; set; } = new ObservableCollection<PriceRangeMarkup>();
    }
}
