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
    /// Группа поставщиков, объединённых по ключу (например, по первой букве названия поставщика).
    /// Заголовок группы используется для отображения группировки в UI.
    /// </summary>
    public class SupplierGroup : ObservableCollection<Supplier>
    {
        /// <summary>
        /// Ключ группы (первая буква имени поставщика, либо "#" если имя отсутствует).
        /// </summary>
        public string Key { get; }

        public SupplierGroup(string key, IEnumerable<Supplier> items) : base(items)
        {
            Key = key;
        }
    }

    /// <summary>
    /// ViewModel для управления данными поставщиков.
    /// Предоставляет коллекцию поставщиков, реализует группировку и содержит команды для загрузки, сохранения и удаления данных.
    /// </summary>
    public class SupplierViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция всех поставщиков.
        /// </summary>
        public ObservableCollection<Supplier> Suppliers { get; } = new ObservableCollection<Supplier>();

        /// <summary>
        /// Поставщики, сгруппированные по ключу (например, по первой букве названия).
        /// Используется для отображения группированных данных в интерфейсе.
        /// </summary>
        public IEnumerable<SupplierGroup> GroupedSuppliers
        {
            get
            {
                return Suppliers
                    .GroupBy(s => s.GroupKey)
                    .OrderBy(g => g.Key)
                    .Select(g => new SupplierGroup(g.Key, g))
                    .ToList();
            }
        }

        private Supplier _selectedSupplier;

        /// <summary>
        /// Выбранный поставщик для редактирования.
        /// При изменении этого свойства UI обновляется для отображения деталей выбранного поставщика.
        /// </summary>
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set { _selectedSupplier = value; OnPropertyChanged(); }
        }

        // События для уведомления представления о результатах операций (успех или ошибка).
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды для выполнения операций с данными поставщиков.
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        /// <summary>
        /// Конструктор ViewModel.
        /// Подписывается на изменение коллекции поставщиков для обновления группировки и инициализирует команды.
        /// </summary>
        public SupplierViewModel()
        {
            // Обновляем группировку при изменении коллекции поставщиков.
            Suppliers.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedSuppliers));

            // Инициализация команд:
            // Команда удаления принимает объект Supplier для удаления.
            DeleteCommand = new RelayCommand(async param => await DeleteSupplierAsync(param));
            // Команда сохранения обновляет данные поставщиков в базе.
            SaveCommand = new RelayCommand(async param => await SaveSuppliersAsync());
            // Команда загрузки получает данные поставщиков из базы.
            LoadCommand = new RelayCommand(async param => await LoadSuppliersAsync());
        }

        /// <summary>
        /// Загружает данные поставщиков из базы данных и обновляет коллекцию Suppliers.
        /// </summary>
        public async Task LoadSuppliersAsync()
        {
            try
            {
                var list = await DataService.LoadSuppliersAsync();
                Suppliers.Clear();
                foreach (var item in list.OrderBy(s => s.Name))
                {
                    Suppliers.Add(item);
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие данные поставщиков в базе данных.
        /// При успешном сохранении уведомляет представление через событие Succeeded.
        /// </summary>
        public async Task SaveSuppliersAsync()
        {
            try
            {
                await DataService.SaveSuppliersAsync(Suppliers);
                Succeeded?.Invoke(this, "Данные поставщиков успешно обновлены");
            }
            catch (Exception ex)
            {
                // В случае ошибки уведомляем представление через событие Fail.
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Удаляет выбранного поставщика из коллекции и базы данных.
        /// </summary>
        private async Task DeleteSupplierAsync(object parameter)
        {
            if (parameter is Supplier supplier)
            {
                Suppliers.Remove(supplier);
                await DataService.DeleteSupplierAsync(supplier);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
