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
    /// Группа наценок поставщиков, сгруппированная по ключу (первой букве имени поставщика).
    /// Ключ группы используется для отображения заголовка в пользовательском интерфейсе.
    /// </summary>
    public class SupplierMarkupGroup : ObservableCollection<SupplierMarkup>
    {
        /// <summary>
        /// Ключ группы (первая буква имени поставщика или "#", если имя отсутствует).
        /// </summary>
        public string Key { get; }

        public SupplierMarkupGroup(string key, IEnumerable<SupplierMarkup> items) : base(items)
        {
            Key = key;
        }
    }

    /// <summary>
    /// ViewModel для управления данными наценок поставщиков.
    /// Обеспечивает коллекцию наценок, реализует группировку по первой букве имени поставщика,
    /// а также содержит команды для загрузки и сохранения данных.
    /// </summary>
    public class SupplierMarkupViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция всех наценок поставщиков.
        /// </summary>
        public ObservableCollection<SupplierMarkup> SupplierMarkups { get; } = new ObservableCollection<SupplierMarkup>();

        /// <summary>
        /// Наценки поставщиков, сгруппированные по первой букве имени.
        /// Используется для отображения данных в группированном виде в пользовательском интерфейсе.
        /// </summary>
        public IEnumerable<SupplierMarkupGroup> GroupedSupplierMarkups
        {
            get
            {
                return SupplierMarkups
                    .GroupBy(s => s.GroupKey)
                    .OrderBy(g => g.Key)
                    .Select(g => new SupplierMarkupGroup(g.Key, g))
                    .ToList();
            }
        }

        private SupplierMarkup _selectedSupplierMarkup;

        /// <summary>
        /// Выбранная наценка для редактирования.
        /// При изменении этого свойства обновляются детали, отображаемые в UI.
        /// </summary>
        public SupplierMarkup SelectedSupplierMarkup
        {
            get => _selectedSupplierMarkup;
            set { _selectedSupplierMarkup = value; OnPropertyChanged(); }
        }

        // События для уведомления представления о результате операций (успех или ошибка).
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        /// <summary>
        /// Команда для сохранения данных наценок поставщиков.
        /// </summary>
        public RelayCommand SaveCommand { get; }
        /// <summary>
        /// Команда для загрузки данных наценок поставщиков.
        /// </summary>
        public RelayCommand LoadCommand { get; }

        /// <summary>
        /// Конструктор, инициализирующий коллекцию наценок и команды, а также подписывающийся на обновление группировки.
        /// </summary>
        public SupplierMarkupViewModel()
        {
            // Обновляем группировку при любом изменении коллекции наценок.
            SupplierMarkups.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedSupplierMarkups));

            // Инициализация команд:
            SaveCommand = new RelayCommand(async param => await SaveSupplierMarkupsAsync());
            LoadCommand = new RelayCommand(async param => await LoadSupplierMarkupsAsync());
        }

        /// <summary>
        /// Загружает данные наценок поставщиков из базы данных и обновляет коллекцию.
        /// </summary>
        public async Task LoadSupplierMarkupsAsync()
        {
            try
            {
                var list = await DataService.LoadSupplierMarkupsAsync();
                SupplierMarkups.Clear();
                foreach (var item in list.OrderBy(s => s.SupplierName))
                {
                    SupplierMarkups.Add(item);
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие данные наценок поставщиков в базе данных.
        /// При успешном сохранении уведомляет представление через событие Succeeded.
        /// </summary>
        public async Task SaveSupplierMarkupsAsync()
        {
            try
            {
                await DataService.SaveSupplierMarkupsAsync(SupplierMarkups);
                Succeeded?.Invoke(this, "Данные поставщиков успешно обновлены");
            }
            catch (Exception ex)
            {
                // При ошибке уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
