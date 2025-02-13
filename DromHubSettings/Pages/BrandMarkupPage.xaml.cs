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
using DromHubSettings.Servises;
using DromHubSettings.Models;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrandMarkupsPage : Page
    {
        public MarkupPageViewModel ViewModel { get; } = new MarkupPageViewModel();
        public BrandMarkupsPage()
        {
            this.InitializeComponent();
        }

        // Переопределяем метод OnNavigatedTo для загрузки данных
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadBrandMarkupsAsync();
        }

        private async Task LoadBrandMarkupsAsync()
        {
            // Очистка текущей коллекции (если требуется)
            ViewModel.BrandMarkups.Clear();

            // Получаем данные из БД
            var list = await DataService.GetBrandMarkupsAsync();

            // Сортировка списка по алфавиту по названию бренда
            foreach (var item in list.OrderBy(b => b.BrandName))
            {
                ViewModel.BrandMarkups.Add(item);
            }
        }

        // Обработчик удаления элемента
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BrandMarkup markup)
            {
                // Удаление из коллекции (и, соответственно, обновление в базе PostgreSQL)
                ViewModel.BrandMarkups.Remove(markup);
                await DataService.DeleteBrandMarkupAsync(markup);
            }
        }

        // Обработчик добавления нового элемента через ContentDialog
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBrandMarkupDialog();
            dialog.XamlRoot = this.XamlRoot; // Устанавливаем XamlRoot из текущей страницы
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newBrand = new BrandMarkup { BrandName = dialog.BrandName, Markup = dialog.Markup };
                ViewModel.BrandMarkups.Add(newBrand);
                await DataService.AddBrandMarkupAsync(newBrand);
            }
        }

        // Обновление записи в базе при изменении значения наценки
        private async void MarkupNumberBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is NumberBox numberBox && numberBox.DataContext is BrandMarkup markup)
            {
                await DataService.UpdateBrandMarkupAsync(markup);
            }
        }
    }

    public class MarkupPageViewModel
    {
        public ObservableCollection<BrandMarkup> BrandMarkups { get; set; } = new ObservableCollection<BrandMarkup>();
    }
}
