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
using System.Collections.Generic;

namespace DromHubSettings.ViewModels
{
    // Класс группы для BrandMarkup
    public class BrandMarkupGroup : ObservableCollection<BrandMarkup>
    {
        public string Key { get; }

        public BrandMarkupGroup(string key, IEnumerable<BrandMarkup> items) : base(items)
        {
            Key = key;
        }
    }

    public class MarkupPageViewModel : INotifyPropertyChanged
    {
        // Коллекция наценок по брендам
        public ObservableCollection<BrandMarkup> BrandMarkups { get; } = new ObservableCollection<BrandMarkup>();

        // Свойство, возвращающее сгруппированные наценки (группировка по первой букве BrandName)
        public IEnumerable<BrandMarkupGroup> GroupedBrandMarkups
        {
            get
            {
                return BrandMarkups
                    .GroupBy(b => b.GroupKey)
                    .OrderBy(g => g.Key)
                    .Select(g => new BrandMarkupGroup(g.Key, g))
                    .ToList();
            }
        }

        private BrandMarkup _selectedMarkup;
        // Текущий выбранный для редактирования элемент
        public BrandMarkup SelectedMarkup
        {
            get => _selectedMarkup;
            set { _selectedMarkup = value; OnPropertyChanged(); }
        }

        // События для уведомления View о результате
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }
        public RelayCommand EditCommand { get; }

        public MarkupPageViewModel()
        {
            // Подписка на изменение коллекции, чтобы обновлять группировку при любых изменениях
            BrandMarkups.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedBrandMarkups));

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
                var list = await DataService.LoadBrandMarkupsAsync();
                BrandMarkups.Clear();
                foreach (var item in list.OrderBy(b => b.BrandName))
                {
                    BrandMarkups.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Можно поднять событие об ошибке, если необходимо
                Fail?.Invoke(this, ex.Message);
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
                Succeeded?.Invoke(this, "Данные успешно обновлены");
            }
            catch (Exception ex)
            {
                // При ошибке сохраняем сообщение и поднимаем событие ошибки
                Fail?.Invoke(this, ex.Message);
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
