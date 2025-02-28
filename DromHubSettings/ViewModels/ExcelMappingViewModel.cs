using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DromHubSettings.Helpers;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;

namespace DromHubSettings.ViewModels
{
    class ExcelMappingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ExcelMapping> ExcelMappings { get; } = new ObservableCollection<ExcelMapping>();

        public event EventHandler<string> Fail;

        public RelayCommand ReadCommand { get; }

        public ExcelMappingViewModel()
        {
            ReadCommand = new RelayCommand(async param => ReadExcelMappingsAsync());
        }

        public async Task ReadExcelMappingsAsync()
        {
            try
            {
                var list = await DataService.ReadExcelMappingsAsync();
                ExcelMappings.Clear();
                foreach (var item in list.OrderBy(e => e.Name))
                {
                    ExcelMappings.Add(item);
                }
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
