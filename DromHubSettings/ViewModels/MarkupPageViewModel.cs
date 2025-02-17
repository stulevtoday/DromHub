using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;
using DromHubSettings.Helpers;
using DromHubSettings.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace DromHubSettings.ViewModels
{
    public class MarkupPageViewModel : INotifyPropertyChanged
    {
        // Коллекция наценок по брендам
        public ObservableCollection<BrandMarkup> BrandMarkups { get; } = new ObservableCollection<BrandMarkup>();

        // События для уведомления View о результате сохранения
        public event EventHandler SaveSucceeded;
        public event EventHandler<string> SaveFailed;

        // Асинхронные команды
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        public MarkupPageViewModel()
        {
            // Команда на удаление: принимает текущий объект BrandMarkup
            DeleteCommand = new RelayCommand(async param => await DeleteBrandMarkupAsync(param));
            // Команда на сохранение изменений
            SaveCommand = new RelayCommand(async param => await SaveBrandMarkupsAsync());
            // Команда на загрузку данных
            LoadCommand = new RelayCommand(async param => await LoadBrandMarkupsAsync());
        }

        /// <summary>
        /// Загружает наценки из базы данных и обновляет коллекцию.
        /// </summary>
        public async Task LoadBrandMarkupsAsync()
        {
            try
            {
                var list = await DataService.GetBrandMarkupsAsync();
                BrandMarkups.Clear();
                foreach (var item in list.OrderBy(b => b.BrandName))
                {
                    BrandMarkups.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Можно поднять событие об ошибке, если необходимо
                SaveFailed?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие наценки в базе данных.
        /// </summary>
        public async Task SaveBrandMarkupsAsync()
        {
            try
            {
                await DataService.SaveBrandMarkupsAsync(BrandMarkups);
                // Если сохранение прошло успешно, поднимаем событие успеха
                SaveSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // При ошибке сохраняем сообщение и поднимаем событие ошибки
                SaveFailed?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Удаляет наценку и удаляет её из базы.
        /// </summary>
        private async Task DeleteBrandMarkupAsync(object parameter)
        {
            if (parameter is BrandMarkup markup)
            {
                BrandMarkups.Remove(markup);
                await DataService.DeleteBrandMarkupAsync(markup);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
