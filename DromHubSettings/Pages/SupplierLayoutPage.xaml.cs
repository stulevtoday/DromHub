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
    public sealed partial class SupplierLayoutPage : Page
    {
        public SupplierLayoutPage()
        {
            this.InitializeComponent();
            this.Loaded += SupplierLayoutPage_Loaded;

            // Подписка на события успешного выполнения и ошибки для отображения уведомлений пользователю.
            if (this.DataContext is SupplierLayoutViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// Обработчик загрузки страницы. Загружает данные поставщиков из базы.
        /// </summary>
        private async void SupplierLayoutPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierLayoutViewModel vm)
            {
                await vm.LoadSupplierLayoutsAsync();
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
        /// Сохраняет текущие изменения поставщиков в базе и обновляет данные.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SupplierLayoutViewModel vm)
            {
                await vm.SaveSupplierLayoutsAsync();
                await vm.LoadSupplierLayoutsAsync();
            }
        }
    }
}
