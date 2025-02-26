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
    /// Страница управления наценками по брендам.
    /// Здесь пользователь может просматривать список брендов с их наценками, 
    /// а также добавлять, редактировать или удалять данные.
    /// </summary>
    public sealed partial class BrandMarkupsPage : Page
    {
        public BrandMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += BrandMarkupsPage_Loaded;

            // Подписываемся на события уведомлений из ViewModel
            if (this.DataContext is BrandMarkupViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// При загрузке страницы инициируется загрузка данных из базы.
        /// </summary>
        private async void BrandMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BrandMarkupViewModel vm)
            {
                await vm.LoadBrandMarkupsAsync();
            }
        }

        /// <summary>
        /// Отображает диалоговое окно при успешном выполнении операции.
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
        /// Обработчик кнопки "Добавить наценку".
        /// Открывает диалог для ввода данных новой наценки, затем добавляет её в коллекцию и сохраняет в базе.
        /// </summary>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBrandMarkupDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var newBrandMarkup = new BrandMarkup
                {
                    Id = Guid.NewGuid(),
                    BrandName = dialog.BrandName,
                    Markup = dialog.Markup
                };

                if (this.DataContext is BrandMarkupViewModel vm)
                {
                    vm.BrandMarkups.Add(newBrandMarkup);
                    await DataService.AddBrandMarkupAsync(newBrandMarkup);
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить изменения".
        /// Сохраняет текущие данные наценок в базе данных и обновляет коллекцию.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BrandMarkupViewModel vm)
            {
                await vm.SaveBrandMarkupsAsync();
                await vm.LoadBrandMarkupsAsync();
            }
        }
    }
}
