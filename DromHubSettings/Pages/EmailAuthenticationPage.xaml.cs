using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using DromHubSettings.ViewModels;
using DromHubSettings.Serviсes;
using DromHubSettings.Dialogs;
using System;

namespace DromHubSettings.Pages
{
    /// <summary>
    /// Страница управления настройками почты для импорта и экспорта прайс-листов.
    /// Здесь можно задать учетные данные (почту и пароль) для получения и выгрузки данных.
    /// </summary>
    public sealed partial class EmailAuthenticationPage : Page
    {
        public EmailAuthenticationPage()
        {
            this.InitializeComponent();
            this.Loaded += EmailAuthenticationPage_Loaded;

            // Подписка на события успешного выполнения и ошибки
            if (this.DataContext is EmailAuthenticationViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        private async void EmailAuthenticationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EmailAuthenticationViewModel vm)
            {
                await vm.LoadEmailAuthenticationsAsync();
            }
        }

        /// <summary>
        /// Отображает диалог об успешном выполнении операции.
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
        /// Отображает диалог с сообщением об ошибке.
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
        /// Сохраняет данные аутентификации и обновляет их.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EmailAuthenticationViewModel vm)
            {
                await vm.SaveEmailAuthenticationsAsync();
                await vm.LoadEmailAuthenticationsAsync();
            }
        }

        private void ShowDownloadPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DownloadPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        private void ShowDownloadPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DownloadPasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
        }

        private void ShowUploadPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UploadPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        private void ShowUploadPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UploadPasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }
}
