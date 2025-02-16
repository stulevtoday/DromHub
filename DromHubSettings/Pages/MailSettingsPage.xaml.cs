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
using DromHubSettings.Servi�es;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DromHubSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MailSettingsPage : Page
    {
        public MailSettingsViewModel ViewModel { get; } = new MailSettingsViewModel();

        public MailSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadSettingsAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ViewModel.SaveSettingsAsync();
                var successDialog = new ContentDialog
                {
                    Title = "���������",
                    Content = "��������� ������� ���������.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (System.Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "������ ��� ����������",
                    Content = "��������� ������: " + ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
        private void ShowDownloadPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DownloadPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        private void ShowDownloadPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DownloadPasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden; // ��� .Peek, ���� ������ �������� ������
        }
        private void ShowUploadPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UploadPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        private void ShowUploadPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UploadPasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden; // ��� .Peek, ���� ������ �������� ������
        }

    }
    public class MailSettingsViewModel
    {
        public MailSettings Settings { get; set; }

        public async Task LoadSettingsAsync()
        {
            Settings = await DataService.GetMailSettingsAsync();
            if (Settings == null)
            {
                Settings = new MailSettings
                {
                    Id = System.Guid.NewGuid(),
                    DownloadEmail = "",
                    DownloadPassword = "",
                    UploadEmail = "",
                    UploadPassword = ""
                };
                // ���� �����, ����� �������� ����� ��� ���������� ����� ������.
            }
        }

        public async Task SaveSettingsAsync()
        {
            await DataService.SaveMailSettingsAsync(Settings);
        }
    }
}