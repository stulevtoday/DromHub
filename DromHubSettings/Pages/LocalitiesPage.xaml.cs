using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DromHubSettings.Models;
using DromHubSettings.Servi�es;
using DromHubSettings.Dialogs;
using DromHubSettings.ViewModels;

namespace DromHubSettings.Pages
{
    /// <summary>
    /// �������� ���������� �������������.
    /// ����� ������������ ����� �������������, ���������, ������������� � ������� ������ ������������.
    /// </summary>
    public sealed partial class LocalitiesPage : Page
    {
        public LocalitiesPage()
        {
            this.InitializeComponent();
            this.Loaded += LocalitiesPage_Loaded;

            // �������� �� ������� ��������� ���������� � ������ ��� ����������� ����������� ������������.
            if (this.DataContext is LocalityViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        /// <summary>
        /// ���������� �������� ��������.
        /// ��������� ������ ������������ �� ���� ������.
        /// </summary>
        private async void LocalitiesPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LocalityViewModel vm)
            {
                await vm.LoadLocalitiesAsync();
            }
        }

        /// <summary>
        /// ���������� ���������� ���� �� �������� ���������� ��������.
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
        /// ���������� ������ "�������� �����������".
        /// ��������� ���������� ���� ��� ����� ������ ����� �����������,
        /// ����� ��������� � � ��������� � ��������� � ����.
        /// </summary>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddLocalityDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var newLocality = new Locality
                {
                    Id = Guid.NewGuid(),
                    Name = dialog.Name,
                    Email = dialog.Email,
                    DeliveryTime = dialog.DeliveryTime
                };

                if (this.DataContext is LocalityViewModel vm)
                {
                    vm.Localities.Add(newLocality);
                    await DataService.AddLocalityAsync(newLocality);
                }
            }
        }

        /// <summary>
        /// ���������� ������ "��������� ���������".
        /// ��������� ������� ��������� ������������ � ���� ������ � ��������� ������.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LocalityViewModel vm)
            {
                await vm.SaveLocalitiesAsync();
                await vm.LoadLocalitiesAsync();
            }
        }
    }
}
