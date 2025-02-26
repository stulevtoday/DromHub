using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DromHubSettings.Helpers;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;

namespace DromHubSettings.ViewModels
{
    /// <summary>
    /// Группа диапазонов цен, объединённая по минимальной и максимальной цене.
    /// Заголовок группы формируется как "minPrice - maxPrice".
    /// </summary>
    public class RangeMarkupGroup : ObservableCollection<RangeMarkup>
    {
        public string Key { get; }

        public RangeMarkupGroup(double minPrice, double maxPrice, IEnumerable<RangeMarkup> items)
            : base(items)
        {
            // Заголовок группы: "minPrice - maxPrice"
            Key = $"{minPrice} - {maxPrice}";
        }
    }

    /// <summary>
    /// ViewModel для управления диапазонами цен.
    /// Предоставляет коллекцию диапазонов, сгруппированных по минимальной и максимальной цене,
    /// а также команды для загрузки, сохранения и удаления данных.
    /// </summary>
    public class RangeMarkupViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция всех диапазонов цен.
        /// </summary>
        public ObservableCollection<RangeMarkup> RangeMarkups { get; } = new ObservableCollection<RangeMarkup>();

        /// <summary>
        /// Диапазоны цен, сгруппированные по минимальной и максимальной цене.
        /// </summary>
        public IEnumerable<RangeMarkupGroup> GroupedRangeMarkups
        {
            get
            {
                return RangeMarkups
                    .GroupBy(r => new { r.MinPrice, r.MaxPrice })
                    .Select(g => new RangeMarkupGroup(g.Key.MinPrice, g.Key.MaxPrice, g))
                    .ToList();
            }
        }

        private RangeMarkup _selectedRangeMarkup;

        /// <summary>
        /// Выбранный диапазон цен для редактирования.
        /// </summary>
        public RangeMarkup SelectedRangeMarkup
        {
            get => _selectedRangeMarkup;
            set { _selectedRangeMarkup = value; OnPropertyChanged(); }
        }

        // События для уведомления View о результате операций (успех или ошибка).
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды для работы с диапазонами цен.
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        public RangeMarkupViewModel()
        {
            // При изменении коллекции диапазонов обновляем группировку.
            RangeMarkups.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedRangeMarkups));

            // Инициализация команд:
            // Команда удаления принимает текущий объект RangeMarkup.
            DeleteCommand = new RelayCommand(async param => await DeleteRangeMarkupAsync(param));
            // Команда сохранения обновляет данные диапазонов цен в базе.
            SaveCommand = new RelayCommand(async param => await SaveRangeMarkupsAsync());
            // Команда загрузки получает данные диапазонов цен из базы.
            LoadCommand = new RelayCommand(async param => await LoadRangeMarkupsAsync());
        }

        /// <summary>
        /// Загружает данные диапазонов цен из базы данных и обновляет коллекцию RangeMarkups.
        /// </summary>
        public async Task LoadRangeMarkupsAsync()
        {
            try
            {
                var list = await DataService.LoadRangeMarkupsAsync();
                RangeMarkups.Clear();
                foreach (var item in list.OrderBy(r => r.MaxPrice))
                {
                    RangeMarkups.Add(item);
                }
            }
            catch (Exception ex)
            {
                // При возникновении ошибки уведомляем View через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие данные диапазонов цен в базе данных.
        /// </summary>
        public async Task SaveRangeMarkupsAsync()
        {
            try
            {
                await DataService.SaveRangeMarkupsAsync(RangeMarkups);
                // Если сохранение прошло успешно, уведомляем View через событие Succeeded.
                Succeeded?.Invoke(this, "Данные диапазонов успешно обновлены");
            }
            catch (Exception ex)
            {
                // При возникновении ошибки уведомляем View через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Удаляет выбранный диапазон цен из коллекции и базы данных.
        /// </summary>
        private async Task DeleteRangeMarkupAsync(object parameter)
        {
            if (parameter is RangeMarkup rangeMarkup)
            {
                RangeMarkups.Remove(rangeMarkup);
                await DataService.DeleteRangeMarkupAsync(rangeMarkup);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
