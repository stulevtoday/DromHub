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
    /// <summary>
    /// Группа наценок для брендов, сгруппированная по первой букве названия бренда.
    /// Заголовок группы формируется на основе свойства GroupKey.
    /// </summary>
    public class BrandMarkupGroup : ObservableCollection<BrandMarkup>
    {
        public string Key { get; }

        public BrandMarkupGroup(string key, IEnumerable<BrandMarkup> items) : base(items)
        {
            Key = key;
        }
    }

    /// <summary>
    /// ViewModel для управления наценками по брендам.
    /// Предоставляет коллекцию наценок, реализует группировку по первой букве названия бренда,
    /// а также команды для загрузки, сохранения и удаления данных.
    /// </summary>
    public class BrandMarkupViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция наценок по брендам.
        /// </summary>
        public ObservableCollection<BrandMarkup> BrandMarkups { get; } = new ObservableCollection<BrandMarkup>();

        /// <summary>
        /// Наценки, сгруппированные по первой букве названия бренда.
        /// </summary>
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

        private BrandMarkup _selectedBrandMarkup;
        /// <summary>
        /// Выбранная наценка для редактирования.
        /// </summary>
        public BrandMarkup SelectedBrandMarkup
        {
            get => _selectedBrandMarkup;
            set { _selectedBrandMarkup = value; OnPropertyChanged(); }
        }

        // События для уведомления View о результате операций (успех или ошибка).
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды для работы с данными наценок.
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        public BrandMarkupViewModel()
        {
            // При изменении коллекции наценок обновляем группировку.
            BrandMarkups.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedBrandMarkups));

            // Инициализация команд:
            // Команда удаления удаляет выбранную наценку.
            DeleteCommand = new RelayCommand(async param => await DeleteBrandMarkupAsync(param));
            // Команда сохранения обновляет данные наценок в базе.
            SaveCommand = new RelayCommand(async param => await SaveBrandMarkupsAsync());
            // Команда загрузки получает данные наценок из базы.
            LoadCommand = new RelayCommand(async param => await LoadBrandMarkupsAsync());
        }

        /// <summary>
        /// Загружает наценки из базы данных и обновляет коллекцию BrandMarkups.
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
                // При возникновении ошибки уведомляем View через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие данные наценок в базе данных.
        /// </summary>
        public async Task SaveBrandMarkupsAsync()
        {
            try
            {
                await DataService.SaveBrandMarkupsAsync(BrandMarkups);
                // Если сохранение прошло успешно, уведомляем View через событие Succeeded.
                Succeeded?.Invoke(this, "Данные успешно обновлены");
            }
            catch (Exception ex)
            {
                // При возникновении ошибки уведомляем View через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Удаляет выбранную наценку из коллекции и базы данных.
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
