

using System;
using System.Windows.Input;

namespace FileManager.WPF.Command
{
    /// <summary>Класс реализующий интерфейс ICommand для создания WPF команд</summary>
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _execute.Invoke();
        }
    }
}
