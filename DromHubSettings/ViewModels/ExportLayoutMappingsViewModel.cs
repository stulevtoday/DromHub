using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DromHubSettings.ViewModels
{
    public class ExportLayoutViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ExportLayoutMapping> Mappings { get; } = new ObservableCollection<ExportLayoutMapping>();

        private ExportLayoutSettings _settings;
        public ExportLayoutSettings Settings
        {
            get => _settings;
            set { _settings = value; OnPropertyChanged(); }
        }

        public async Task LoadAsync()
        {
            // Загрузим маппинги (предположим, что они уже реализованы)
            var mappings = await DataService.GetExportLayoutMappingsAsync();
            Mappings.Clear();
            foreach (var mapping in mappings)
            {
                Mappings.Add(mapping);
            }
            // Загрузим настройки экспорта
            Settings = await DataService.GetExportLayoutSettingsAsync();
            // Если настроек нет, можно создать новый объект с дефолтным значением, например, StartRow = 2
            if (Settings == null)
            {
                Settings = new ExportLayoutSettings
                {
                    Id = System.Guid.NewGuid(),
                    StartRow = 2
                };
            }
        }

        public async Task SaveAsync()
        {
            await DataService.SaveExportLayoutMappingsAsync(Mappings);
            await DataService.SaveExportLayoutSettingsAsync(Settings);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
