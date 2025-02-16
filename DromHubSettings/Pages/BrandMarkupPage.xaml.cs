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
using DromHubSettings.Servi�es;

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

        // �������������� ����� OnNavigatedTo ��� �������� ������
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadBrandMarkupsAsync();
        }

        private async Task LoadBrandMarkupsAsync()
        {
            // ������� ������� ��������� (���� ���������)
            ViewModel.BrandMarkups.Clear();

            // �������� ������ �� ��
            var list = await DataService.GetBrandMarkupsAsync();

            // ���������� ������ �� �������� �� �������� ������
            foreach (var item in list.OrderBy(b => b.BrandName))
            {
                ViewModel.BrandMarkups.Add(item);
            }
        }

        // ���������� �������� ��������
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BrandMarkup markup)
            {
                // �������� �� ��������� (�, ��������������, ���������� � ���� PostgreSQL)
                ViewModel.BrandMarkups.Remove(markup);
                await DataService.DeleteBrandMarkupAsync(markup);
            }
        }

        // ���������� ���������� ������ �������� ����� ContentDialog
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBrandMarkupDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newBrand = new BrandMarkup
                {
                    Id = Guid.NewGuid(), // ���������� ����� id �� ������� �������
                    BrandName = dialog.BrandName,
                    Markup = dialog.Markup
                };
                ViewModel.BrandMarkups.Add(newBrand);
                await DataService.AddBrandMarkupAsync(newBrand);
            }
        }

        // ���������� ������ � ���� ��� ��������� �������� �������
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // �����������, ��� � ��� ���� DataService ��� ������ � ����� ������.
                // ��������, DataService.UpdateBrandMarkupsAsync ��������� ��������� ���������� ������.
                await DataService.SaveBrandMarkupsAsync(ViewModel.BrandMarkups);

                var successDialog = new ContentDialog
                {
                    Title = "���������",
                    Content = "������ ������� ���������.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
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
    }

    public class MarkupPageViewModel
    {
        public ObservableCollection<BrandMarkup> BrandMarkups { get; set; } = new ObservableCollection<BrandMarkup>();
    }
}
