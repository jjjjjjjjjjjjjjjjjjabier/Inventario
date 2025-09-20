// Archivo: Programa/InventarioComputo/InventarioComputo.UI/Commands/RelayCommand.cs
using System;
using System.Windows.Input;

namespace InventarioComputo.UI.Commands
{
    /// <summary>
    /// Una implementación de ICommand para comandos que no requieren un parámetro fuertemente tipado.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Una implementación genérica de ICommand para comandos que requieren un parámetro fuertemente tipado.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is T typedParam)
            {
                return _canExecute == null || _canExecute(typedParam);
            }
            if (parameter == null && default(T) == null)
            {
                return _canExecute == null || _canExecute(default(T));
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is T typedParam)
            {
                _execute(typedParam);
            }
            else if (parameter == null && default(T) == null)
            {
                _execute(default(T));
            }
        }

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}