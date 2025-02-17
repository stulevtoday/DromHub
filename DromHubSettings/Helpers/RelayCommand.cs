using System;
using System.Windows.Input;

namespace DromHubSettings.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object, System.Threading.Tasks.Task> _executeAsync;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Func<object, System.Threading.Tasks.Task> executeAsync, Predicate<object> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public async void Execute(object parameter)
        {
            await _executeAsync(parameter);
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
