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
using DromHubSettings.Serviсes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DromHubSettings.Dialogs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalitiesPage : Page
    {
        public LocalitiesViewModel ViewModel { get; } = new LocalitiesViewModel();

        public LocalitiesPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadLocalitiesAsync();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddLocalityDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newLocality = new Models.LocalityOption
                {
                    Id = System.Guid.NewGuid(),
                    Name = dialog.LocalityName,
                    DeliveryTime = dialog.DeliveryTime,
                    ExportEmail = dialog.ExportEmail
                };

                await DataService.AddLocalityAsync(newLocality);
                ViewModel.Localities.Add(newLocality);
            }
        }


        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Models.LocalityOption locality)
            {
                try
                {
                    ViewModel.Localities.Remove(locality);
                    await DataService.DeleteLocalityAsync(locality);
                }
                catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
                {
                    // 23503 – нарушение внешнего ключа
                    var errorDialog = new ContentDialog
                    {
                        Title = "Ошибка",
                        Content = "Невозможно обновить или удалить локальность, так как она используется некоторыми поставщиками. Пожалуйста, измените или удалите связанные записи.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                    this.Frame.Navigate(typeof(LocalitiesPage));
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Ошибка",
                        Content = "Произошла ошибка: " + ex.Message,
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Предположим, что у вас есть DataService для работы с базой данных.
                // Например, DataService.UpdateBrandMarkupsAsync принимает коллекцию обновлённых данных.
                await DataService.SaveLocalitiesAsync(ViewModel.Localities);

                var successDialog = new ContentDialog
                {
                    Title = "Сохранено",
                    Content = "Данные успешно обновлены.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            {
                // 23503 – нарушение внешнего ключа
                var errorDialog = new ContentDialog
                {
                    Title = "Ошибка при сохранении",
                    Content = "Невозможно обновить или удалить локальность, так как она используется некоторыми поставщиками. Пожалуйста, измените или удалите связанные записи.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
                this.Frame.Navigate(typeof(LocalitiesPage));
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

    public class LocalitiesViewModel
    {
        public ObservableCollection<LocalityOption> Localities { get; } = new ObservableCollection<LocalityOption>();

        public async Task LoadLocalitiesAsync()
        {
            var localities = await DataService.GetLocalityOptionsAsync();
            Localities.Clear();
            foreach (var loc in localities)
            {
                Localities.Add(loc);
            }
        }
    }
}
