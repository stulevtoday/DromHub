using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using DromHubSettings.Models;
using DromHubSettings.Serviсes;
using DromHubSettings.Helpers;

namespace DromHubSettings.ViewModels
{
    /// <summary>
    /// ViewModel для управления данными аутентификации электронной почты.
    /// Предоставляет единственный объект EmailAuthentication, обернутый в ObservableCollection для удобства привязки.
    /// </summary>
    public class EmailAuthenticationViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция с единственным объектом EmailAuthentication для привязки в UI.
        /// </summary>
        public ObservableCollection<EmailAuthentication> EmailAuthentications { get; } = new ObservableCollection<EmailAuthentication>();

        // События для уведомления представления о результатах операций (успех или ошибка).
        public event EventHandler<string> Succeeded;
        public event EventHandler<string> Fail;

        // Асинхронные команды для загрузки и сохранения данных.
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        /// <summary>
        /// Конструктор ViewModel.
        /// Инициализирует команды и добавляет дефолтный объект, если коллекция пуста.
        /// </summary>
        public EmailAuthenticationViewModel()
        {
            // Добавляем дефолтный объект, если коллекция пуста.
            if (EmailAuthentications.Count == 0)
            {
                EmailAuthentications.Add(new EmailAuthentication());
            }

            SaveCommand = new RelayCommand(async param => await SaveEmailAuthenticationsAsync());
            LoadCommand = new RelayCommand(async param => await LoadEmailAuthenticationsAsync());
        }

        /// <summary>
        /// Загружает данные аутентификации электронной почты из базы данных.
        /// Ожидается, что DataService.LoadEmailAuthenticationsAsync возвращает единственный объект EmailAuthentication.
        /// Если объект не равен null, он добавляется в коллекцию EmailAuthentications.
        /// </summary>
        public async Task LoadEmailAuthenticationsAsync()
        {
            try
            {
                var auth = await DataService.LoadEmailAuthenticationsAsync(); // auth имеет тип EmailAuthentication
                EmailAuthentications.Clear();
                if (auth != null)
                {
                    EmailAuthentications.Add(auth);
                }
                else
                {
                    // Если в БД нет записи, добавляем "пустой" объект
                    EmailAuthentications.Add(new EmailAuthentication());
                }
            }
            catch (Exception ex)
            {
                Fail?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет данные аутентификации электронной почты в базе данных.
        /// Если в коллекции EmailAuthentications содержится объект, он передается в DataService.SaveEmailAuthenticationsAsync.
        /// </summary>
        public async Task SaveEmailAuthenticationsAsync()
        {
            try
            {
                var auth = EmailAuthentications.Count > 0 ? EmailAuthentications[0] : null;
                await DataService.SaveEmailAuthenticationsAsync(auth);
                Succeeded?.Invoke(this, "Данные успешно обновлены");
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
