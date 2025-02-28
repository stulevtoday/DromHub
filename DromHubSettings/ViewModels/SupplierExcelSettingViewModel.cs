using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DromHubSettings.Helpers;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;

namespace DromHubSettings.ViewModels
{
    /// <summary>
    /// Группа поставщиков, объединённых по ключу (например, по первой букве названия поставщика).
    /// Заголовок группы используется для отображения группировки в UI.
    /// </summary>
    public class SupplierExcelSettingGroup : ObservableCollection<SupplierExcelSetting>
    {
        /// <summary>
        /// Ключ группы (первая буква имени поставщика, либо "#" если имя отсутствует).
        /// </summary>
        public string Key { get; }

        public SupplierExcelSettingGroup(string key, IEnumerable<SupplierExcelSetting> items) : base(items)
        {
            Key = key;
        }
    }
    public class SupplierExcelSettingViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Сгруппированные сопоставления по ключу, который определяется по первой букве имени поставщика.
        /// Это свойство используется для отображения данных в группированном виде.
        /// </summary>
        public IEnumerable<SupplierExcelSettingGroup> GroupedSupplierExcelSettings
        {
            get
            {
                return SupplierExcelSettings
                    .GroupBy(s => s.GroupKey)
                    .OrderBy(g => g.Key)
                    .Select(g => new SupplierExcelSettingGroup(g.Key, g))
                    .ToList();
            }
        }

        private SupplierExcelSetting _selectedSupplierExcelSetting;

        /// <summary>
        /// Выбранное сопоставление для редактирования.
        /// При установке этого свойства обновляются детали, отображаемые в форме редактирования.
        /// </summary>
        public SupplierExcelSetting SelectedSupplierExcelSetting
        {
            get => _selectedSupplierExcelSetting;
            set { _selectedSupplierExcelSetting = value; OnPropertyChanged(); }
        }
        public ObservableCollection<SupplierExcelSetting> SupplierExcelSettings { get; } = new ObservableCollection<SupplierExcelSetting>();

        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        public RelayCommand UpdateCommand { get; }
        public RelayCommand ReadCommand { get; }

        public SupplierExcelSettingViewModel()
        {
            SupplierExcelSettings.CollectionChanged += (s, e) => OnPropertyChanged(nameof(GroupedSupplierExcelSettings));
            UpdateCommand = new RelayCommand(async param => await UpdateSupplierExcelSettingsAsync());
            ReadCommand = new RelayCommand(async param => await ReadSupplierExcelSettingsAsync());
        }

        public async Task ReadSupplierExcelSettingsAsync()
        {
            try
            {
                var list = await DataService.ReadSupplierExcelSettingsAsync();
                SupplierExcelSettings.Clear();
                foreach (var item in list.OrderBy(s => s.SupplierName))
                {
                    SupplierExcelSettings.Add(item);
                }
            }
            catch (Exception ex)
            {
                Fail?.Invoke(this, ex.Message);
            }
        }

        public async Task UpdateSupplierExcelSettingsAsync()
        {
            try
            {
                await DataService.UpdateSupplierExcelSettingsAsync(SupplierExcelSettings);
                Succeeded?.Invoke(this, "Данные настроек Excel-файлов поставщика успешно обновлены");
            }
            catch (Exception ex)
            {
                Fail?.Invoke(this, ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
