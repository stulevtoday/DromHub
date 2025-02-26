using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using DromHubSettings.Dialogs;
using DromHubSettings.ViewModels;

namespace DromHubSettings.Pages
{
    public sealed partial class SupplierMarkupsPage : Page
    {
        public SupplierMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += SupplierMarkupsPage_Loaded;

            // Подписка на события успешного выполнения и ошибки для отображения уведомлений пользователю.
            if (this.DataContext is SupplierViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// Обработчик загрузки страницы. Загружает данные наценок поставщиков из базы.
        /// </summary>
        private async void SupplierMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierMarkupViewModel vm)
            {
                await vm.LoadSupplierMarkupsAsync();
            }
        }

        /// <summary>
        /// Отображает диалоговое окно об успешном выполнении операции.
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
        /// Обработчик кнопки "Сохранить изменения".
        /// Сохраняет текущие изменения наценок поставщиков в базе и обновляет данные.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierMarkupViewModel vm)
            {
                await vm.SaveSupplierMarkupsAsync();
                await vm.LoadSupplierMarkupsAsync();
            }
        }
    }
}
