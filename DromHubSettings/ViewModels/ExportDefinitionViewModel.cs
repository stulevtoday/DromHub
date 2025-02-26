using DromHubSettings.Helpers;
using DromHubSettings.Models;
using DromHubSettings.Serviсes; // Здесь находятся методы для работы с БД
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DromHubSettings.ViewModels
{
    public class ExportDefinitionViewModel : INotifyPropertyChanged
    {
        // Объект ExportLayout, загруженный из БД
        private ExportLayout _layout;

        public event PropertyChangedEventHandler PropertyChanged;
        // События для уведомления UI об успехе или ошибке
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Команды для работы с данными (аналогично SupplierViewModel)
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }
        public RelayCommand AddColumnCommand { get; }
        public RelayCommand DeleteColumnCommand { get; }

        public ExportDefinitionViewModel()
        {
            // Коллекция столбцов для биндинга в UI
            Columns = new ObservableCollection<ExportColumnDefinition>();

            // Инициализация команд
            SaveCommand = new RelayCommand(async param => await SaveExportLayoutAsync());
            LoadCommand = new RelayCommand(async param => await LoadExportLayoutAsync());
            AddColumnCommand = new RelayCommand(param => AddColumn());
            DeleteColumnCommand = new RelayCommand(async param => await DeleteColumnAsync(param));
        }

        // Начальная строка для экспорта
        private int _startRow;
        public int StartRow
        {
            get => _startRow;
            set
            {
                if (_startRow != value)
                {
                    _startRow = value;
                    OnPropertyChanged();
                }
            }
        }

        // Коллекция столбцов экспорта (например, "Артикул", "Бренд" и т.д.)
        public ObservableCollection<ExportColumnDefinition> Columns { get; }

        /// <summary>
        /// Загружает настройки экспорта (ExportLayout и связанные столбцы) из БД.
        /// </summary>
        public async Task LoadExportLayoutAsync()
        {
            try
            {
                var layout = await DataService.LoadExportLayoutAsync();
                if (layout == null)
                {
                    // Если в БД нет настроек, создаём новую "пустую" раскладку
                    layout = new ExportLayout
                    {
                        Id = Guid.NewGuid(),
                        StartRow = 1,
                        Columns = new List<ExportColumnDefinition>()
                    };
                }

                _layout = layout;
                StartRow = layout.StartRow;

                Columns.Clear();
                foreach (var col in layout.Columns)
                {
                    Columns.Add(col);
                }

                Succeeded?.Invoke(this, "Настройки экспорта загружены.");
            }
            catch (Exception ex)
            {
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет текущие настройки экспорта (ExportLayout и связанные столбцы) в БД.
        /// Логика разделена на обновление существующих столбцов, добавление новых и удаление удалённых.
        /// </summary>
        public async Task SaveExportLayoutAsync()
        {
            try
            {
                // Обновляем начальную строку в раскладке
                _layout.StartRow = StartRow;
                // Сохраняем общую запись ExportLayout
                await DataService.SaveExportLayoutAsync(_layout);

                // Создаем копию исходного списка столбцов, загруженных из БД
                var originalColumns = new List<ExportColumnDefinition>(_layout.Columns);
                // Текущее состояние столбцов, заданное пользователем
                var newColumns = new List<ExportColumnDefinition>(this.Columns);

                // Обрабатываем новые и обновленные столбцы:
                // Если столбец уже существует в оригинале – обновляем его,
                // иначе – устанавливаем родительский Id и добавляем в БД.
                foreach (var col in newColumns)
                {
                    if (originalColumns.Any(x => x.Id == col.Id))
                    {
                        await DataService.SaveExportColumnDefinitionAsync(col);
                    }
                    else
                    {
                        col.ExportLayoutId = _layout.Id;
                        await DataService.AddExportColumnDefinitionAsync(col);
                    }
                }

                // Обрабатываем удаленные столбцы:
                // Если столбец из исходного списка отсутствует в новом – удаляем его.
                foreach (var col in originalColumns)
                {
                    if (!newColumns.Any(x => x.Id == col.Id))
                    {
                        await DataService.DeleteExportColumnDefinitionAsync(col);
                    }
                }

                // Обновляем список столбцов в объекте _layout
                _layout.Columns = newColumns;

                Succeeded?.Invoke(this, "Настройки экспорта успешно сохранены.");
            }
            catch (Exception ex)
            {
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Добавляет новый столбец с дефолтными значениями в коллекцию.
        /// </summary>
        public void AddColumn()
        {
            var newColumn = new ExportColumnDefinition
            {
                Id = Guid.NewGuid(),
                ColumnName = "Новый столбец",
                ColumnIndex = Columns.Count + 1,
                IsVisible = true,
                MappingProperty = ""
            };

            Columns.Add(newColumn);
        }

        /// <summary>
        /// Удаляет выбранный столбец из коллекции и (опционально) из БД.
        /// </summary>
        public async Task DeleteColumnAsync(object parameter)
        {
            if (parameter is ExportColumnDefinition column)
            {
                // Удаляем из коллекции
                Columns.Remove(column);
                // Немедленно удаляем из БД
                await DataService.DeleteExportColumnDefinitionAsync(column);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
