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
    /// Представляет группу сопоставлений Excel разметки для поставщиков,
    /// сгруппированных по первому символу имени поставщика.
    /// Заголовок группы (Key) используется для отображения группы в пользовательском интерфейсе.
    /// </summary>
    public class SupplierLayoutGroup : ObservableCollection<SupplierLayout>
    {
        /// <summary>
        /// Ключ группы, например, первая буква имени поставщика.
        /// </summary>
        public string Key { get; }

        public SupplierLayoutGroup(string key, IEnumerable<SupplierLayout> items)
            : base(items)
        {
            Key = key;
        }
    }

    /// <summary>
    /// ViewModel для управления сопоставлениями столбцов Excel файлов поставщиков.
    /// Этот класс загружает, сохраняет и обновляет коллекцию сопоставлений, а также уведомляет представление об успехе или ошибке.
    /// </summary>
    public class SupplierLayoutViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция сопоставлений, содержащая информацию о том, какой номер столбца соответствует какому полю поставщика.
        /// </summary>
        public ObservableCollection<SupplierLayout> SupplierLayouts { get; } = new ObservableCollection<SupplierLayout>();

        /// <summary>
        /// Сгруппированные сопоставления по ключу, который определяется по первой букве имени поставщика.
        /// Это свойство используется для отображения данных в группированном виде.
        /// </summary>
        public IEnumerable<SupplierLayoutGroup> GroupedSupplierLayouts
        {
            get
            {
                return SupplierLayouts
                    .GroupBy(s => s.GroupKey)
                    .OrderBy(g => g.Key)
                    .Select(g => new SupplierLayoutGroup(g.Key, g))
                    .ToList();
            }
        }

        private SupplierLayout _selectedSupplierExcelMapping;

        /// <summary>
        /// Выбранное сопоставление для редактирования.
        /// При установке этого свойства обновляются детали, отображаемые в форме редактирования.
        /// </summary>
        public SupplierLayout SelectedSupplierExcelMapping
        {
            get => _selectedSupplierExcelMapping;
            set { _selectedSupplierExcelMapping = value; OnPropertyChanged(); }
        }

        // События для уведомления представления о результате операций.
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды для загрузки, сохранения и, при необходимости, удаления сопоставлений.
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        /// <summary>
        /// Конструктор, инициализирующий ViewModel и настраивающий команды.
        /// Также подписывается на изменения коллекции для обновления группировки.
        /// </summary>
        public SupplierLayoutViewModel()
        {
            // При любом изменении коллекции обновляем отображаемую группировку.
            SupplierLayouts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedSupplierLayouts));

            // Инициализация команд.
            // Команда сохранения вызывает метод сохранения сопоставлений в базе.
            SaveCommand = new RelayCommand(async param => await SaveSupplierLayoutsAsync());
            // Команда загрузки вызывает метод получения сопоставлений из базы.
            LoadCommand = new RelayCommand(async param => await LoadSupplierLayoutsAsync());
        }

        /// <summary>
        /// Загружает сопоставления Excel разметки из базы данных и обновляет коллекцию SupplierExcelMappings.
        /// </summary>
        public async Task LoadSupplierLayoutsAsync()
        {
            try
            {
                var list = await DataService.LoadSupplierLayoutsAsync();
                SupplierLayouts.Clear();
                foreach (var mapping in list.OrderBy(s => s.SupplierName))
                {
                    SupplierLayouts.Add(mapping);
                }
            }
            catch (Exception ex)
            {
                // Если возникла ошибка, уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие сопоставления Excel разметки в базе данных.
        /// При успешном сохранении вызывается событие Succeeded.
        /// </summary>
        public async Task SaveSupplierLayoutsAsync()
        {
            try
            {
                await DataService.SaveSupplierLayoutsAsync(SupplierLayouts);
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
