using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DromHubSettings.ViewModels;
using DromHubSettings.Models;
using DromHubSettings.Dialogs;
using System;

namespace DromHubSettings.Pages
{
    public sealed partial class ExportDefinitionPage : Page
    {
        public ExportDefinitionPage()
        {
            this.InitializeComponent();

            // Подписываемся на события ViewModel (успех/ошибка)
            if (this.DataContext is ExportDefinitionViewModel vm)
            {
                vm.Succeeded += Vm_Succeeded;
                vm.Fail += Vm_Fail;
            }

            this.Loaded += ExportDefinitionPage_Loaded;
        }

        private async void ExportDefinitionPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ExportDefinitionViewModel vm)
            {
                await vm.LoadExportLayoutAsync();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ExportDefinitionViewModel vm)
            {
                await vm.SaveExportLayoutAsync();
            }
        }

        private async void AddColumnButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ExportDefinitionViewModel vm)
            {
                // Открываем диалог
                var dialog = new AddExportColumnDialog();
                dialog.XamlRoot = this.XamlRoot; // для WinUI
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Добавляем столбец в коллекцию VM
                    vm.Columns.Add(new ExportColumnDefinition
                    {
                        Id = Guid.NewGuid(),
                        ColumnName = dialog.ColumnName,
                        ColumnIndex = dialog.ColumnIndex,
                        MappingProperty = dialog.MappingProperty,
                        IsVisible = dialog.IsVisible
                    });
                }
            }
        }

        private void RemoveColumnButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ExportDefinitionViewModel vm)
            {
                var selected = ColumnsListView.SelectedItem as ExportColumnDefinition;
                if (selected != null)
                {
                    vm.Columns.Remove(selected);
                }
            }
        }

        private async void Vm_Succeeded(object sender, string content)
        {
            var dialog = new ContentDialog
            {
                Title = "Успешно",
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async void Vm_Fail(object sender, string content)
        {
            var dialog = new ContentDialog
            {
                Title = "Ошибка",
                Content = "Ошибка: " + content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
