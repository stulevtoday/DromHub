using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using DromHubSettings.ViewModels;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;
using DromHubSettings.Dialogs;

namespace DromHubSettings.Pages
{
    /// <summary>
    /// Страница управления диапазонами цен.
    /// Здесь можно просматривать, добавлять, редактировать и удалять диапазоны цен и соответствующую наценку.
    /// </summary>
    public sealed partial class RangeMarkupsPage : Page
    {
        public RangeMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += RangeMarkupsPage_Loaded;

            // Подписка на события успешного выполнения и ошибки для отображения уведомлений пользователю.
            if (this.DataContext is RangeMarkupViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// Обработчик события загрузки страницы.
        /// Загружает диапазоны цен из базы данных при инициализации страницы.
        /// </summary>
        private async void RangeMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RangeMarkupViewModel vm)
            {
                await vm.LoadRangeMarkupsAsync();
            }
        }

        /// <summary>
        /// Обработчик события успешного выполнения операции.
        /// Отображает диалоговое окно с сообщением об успехе.
        /// </summary>
        private async void Vm_Succeeded(object sender, string content)
        {
            var successDialog = new ContentDialog
            {
                Title = "Успешно",
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        /// <summary>
        /// Обработчик события ошибки.
        /// Отображает диалоговое окно с сообщением об ошибке.
        /// </summary>
        private async void Vm_Fail(object sender, string content)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Ошибка",
                Content = "Ошибка: " + content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        /// <summary>
        /// Обработчик кнопки "Добавить диапазон".
        /// Открывает диалог для ввода нового диапазона цен, затем добавляет его в коллекцию и сохраняет в базу.
        /// </summary>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddRangeMarkupDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var newRangeMarkup = new RangeMarkup
                {
                    Id = Guid.NewGuid(),
                    MinPrice = dialog.MinPrice,
                    MaxPrice = dialog.MaxPrice,
                    Markup = dialog.Markup
                };

                if (this.DataContext is RangeMarkupViewModel vm)
                {
                    vm.RangeMarkups.Add(newRangeMarkup);
                    await DataService.AddRangeMarkupAsync(newRangeMarkup);
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить изменения".
        /// Сохраняет текущие диапазоны цен в базе данных и обновляет список.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RangeMarkupViewModel vm)
            {
                await vm.SaveRangeMarkupsAsync();
                await vm.LoadRangeMarkupsAsync();
            }
        }
    }
}
