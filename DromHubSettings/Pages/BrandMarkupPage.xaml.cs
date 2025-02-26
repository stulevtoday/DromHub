using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using DromHubSettings.ViewModels;
using DromHubSettings.Models;
using DromHubSettings.Servi�es;
using DromHubSettings.Dialogs;

namespace DromHubSettings.Pages
{
    /// <summary>
    /// �������� ���������� ��������� �� �������.
    /// ����� ������������ ����� ������������� ������ ������� � �� ���������, 
    /// � ����� ���������, ������������� ��� ������� ������.
    /// </summary>
    public sealed partial class BrandMarkupsPage : Page
    {
        public BrandMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += BrandMarkupsPage_Loaded;

            // ������������� �� ������� ����������� �� ViewModel
            if (this.DataContext is BrandMarkupViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ��� �������� �������� ������������ �������� ������ �� ����.
        /// </summary>
        private async void BrandMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BrandMarkupViewModel vm)
            {
                await vm.LoadBrandMarkupsAsync();
            }
        }

        /// <summary>
        /// ���������� ���������� ���� ��� �������� ���������� ��������.
        /// </summary>
        private async void Vm_Succeeded(object sender, string content)
        {
            var successDialog = new ContentDialog
            {
                Title = "�������",
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        /// <summary>
        /// ���������� ���������� ���� � ���������� �� ������.
        /// </summary>
        private async void Vm_Fail(object sender, string content)
        {
            var errorDialog = new ContentDialog
            {
                Title = "������",
                Content = "������: " + content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        /// <summary>
        /// ���������� ������ "�������� �������".
        /// ��������� ������ ��� ����� ������ ����� �������, ����� ��������� � � ��������� � ��������� � ����.
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
        /// ���������� ������ "��������� ���������".
        /// ��������� ������� ������ ������� � ���� ������ � ��������� ���������.
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
