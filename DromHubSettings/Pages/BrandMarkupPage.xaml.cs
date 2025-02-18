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
    public sealed partial class BrandMarkupsPage : Page
    {
        public BrandMarkupsPage()
        {
            this.InitializeComponent();
            this.Loaded += BrandMarkupsPage_Loaded;

            if (this.DataContext is MarkupPageViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }
        }

        private async void BrandMarkupsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MarkupPageViewModel vm)
            {
                await vm.LoadBrandMarkupsAsync();
            }
        }

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

        private async void Vm_Fail(object sender, string content)
        {
            var successDialog = new ContentDialog
            {
                Title = "Неудача",
                Content = "Ошибка - " + content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await successDialog.ShowAsync();
        }

        // Обработчик кнопки "Добавить бренд" в Code-behind
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBrandMarkupDialog();
            dialog.XamlRoot = this.XamlRoot;
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var newBrand = new BrandMarkup
                {
                    Id = Guid.NewGuid(),
                    BrandName = dialog.BrandName,
                    Markup = dialog.Markup
                };

                if (this.DataContext is MarkupPageViewModel vm)
                {
                    vm.BrandMarkups.Add(newBrand);
                    await DataService.AddBrandMarkupAsync(newBrand);
                }
            }
        }

        // Обработчик кнопки "Сохранить изменения"
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MarkupPageViewModel vm)
            {
                await vm.SaveBrandMarkupsAsync();
            }
        }
    }
}
