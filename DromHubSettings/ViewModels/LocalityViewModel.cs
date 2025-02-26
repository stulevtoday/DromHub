using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Группа локальностей, объединённых по ключу (например, по первой букве названия).
    /// Ключ группы используется для отображения заголовка группировки в пользовательском интерфейсе.
    /// </summary>
    public class LocalityGroup : ObservableCollection<Locality>
    {
        /// <summary>
        /// Ключ группы (первая буква названия или "#" если название отсутствует).
        /// </summary>
        public string Key { get; }

        public LocalityGroup(string key, IEnumerable<Locality> items) : base(items)
        {
            Key = key;
        }
    }

    /// <summary>
    /// ViewModel для управления данными локальностей.
    /// Обеспечивает коллекцию локальностей, реализует их группировку и предоставляет команды для загрузки, сохранения и удаления данных.
    /// </summary>
    public class LocalityViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция всех локальностей.
        /// </summary>
        public ObservableCollection<Locality> Localities { get; } = new ObservableCollection<Locality>();

        /// <summary>
        /// Локальности, сгруппированные по первой букве названия.
        /// Используется для отображения данных в группированном виде в интерфейсе.
        /// </summary>
        public IEnumerable<LocalityGroup> GroupedLocalities
        {
            get
            {
                return Localities
                    .GroupBy(l => l.GroupKey)
                    .OrderBy(g => g.Key)
                    .Select(g => new LocalityGroup(g.Key, g))
                    .ToList();
            }
        }

        private Locality _selectedLocality;

        /// <summary>
        /// Выбранная локальность для редактирования.
        /// При изменении этого свойства обновляются детали, отображаемые в UI.
        /// </summary>
        public Locality SelectedLocality
        {
            get => _selectedLocality;
            set { _selectedLocality = value; OnPropertyChanged(); }
        }

        // События для уведомления представления о результатах операций (успех или ошибка).
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды для загрузки, сохранения и удаления данных локальностей.
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        /// <summary>
        /// Конструктор LocalityViewModel.
        /// Подписывается на изменения коллекции для обновления группировки и инициализирует команды.
        /// </summary>
        public LocalityViewModel()
        {
            // Обновляем группировку, когда коллекция локальностей изменяется.
            Localities.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedLocalities));

            // Инициализация команд:
            // Команда удаления удаляет выбранную локальность.
            DeleteCommand = new RelayCommand(async param => await DeleteLocalityAsync(param));
            // Команда сохранения обновляет данные локальностей в базе.
            SaveCommand = new RelayCommand(async param => await SaveLocalitiesAsync());
            // Команда загрузки получает данные локальностей из базы.
            LoadCommand = new RelayCommand(async param => await LoadLocalitiesAsync());
        }

        /// <summary>
        /// Загружает данные локальностей из базы данных и обновляет коллекцию Localities.
        /// </summary>
        public async Task LoadLocalitiesAsync()
        {
            try
            {
                var list = await DataService.LoadLocalitiesAsync();
                Localities.Clear();
                foreach (var item in list.OrderBy(l => l.Name))
                {
                    Localities.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Если возникает ошибка, уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие данные локальностей в базе данных.
        /// При успешном сохранении уведомляет представление через событие Succeeded.
        /// </summary>
        public async Task SaveLocalitiesAsync()
        {
            try
            {
                await DataService.SaveLocalitiesAsync(Localities);
                Succeeded?.Invoke(this, "Данные локальностей успешно обновлены");
            }
            catch (Exception ex)
            {
                // Если возникает ошибка, уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Удаляет выбранную локальность из коллекции и базы данных.
        /// </summary>
        private async Task DeleteLocalityAsync(object parameter)
        {
            if (parameter is Locality locality)
            {
                Localities.Remove(locality);
                await DataService.DeleteLocalityAsync(locality);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
