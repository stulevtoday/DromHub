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
    /// �������� ���������� ����������� ���.
    /// ����� ����� �������������, ���������, ������������� � ������� ��������� ��� � ��������������� �������.
    /// </summary>
    public sealed partial class RangeMarkupsPage : Page
    {
        public RangeMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += RangeMarkupsPage_Loaded;

            // �������� �� ������� ��������� ���������� � ������ ��� ����������� ����������� ������������.
            if (this.DataContext is RangeMarkupViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ���������� ������� �������� ��������.
        /// ��������� ��������� ��� �� ���� ������ ��� ������������� ��������.
        /// </summary>
        private async void RangeMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RangeMarkupViewModel vm)
            {
                await vm.LoadRangeMarkupsAsync();
            }
        }

        /// <summary>
        /// ���������� ������� ��������� ���������� ��������.
        /// ���������� ���������� ���� � ���������� �� ������.
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
        /// ���������� ������� ������.
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
        /// ���������� ������ "�������� ��������".
        /// ��������� ������ ��� ����� ������ ��������� ���, ����� ��������� ��� � ��������� � ��������� � ����.
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
        /// ���������� ������ "��������� ���������".
        /// ��������� ������� ��������� ��� � ���� ������ � ��������� ������.
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
