using System;
using System.Windows.Input;

namespace FileManager.WPF.Command
{
    /// <summary>Класс реализующий интерфейс ICommand для создания WPF команд</summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public event EventHandler? CanExecuteChanged;

        /// <summary> Создание экземпляра реле команд. </summary>
        /// <param name="execute"> Выполняемый метод. </param>
        public RelayCommand(Action<object> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
        }

        /// <summary> Проверить, может ли метод быть выполнен. </summary>
        /// <param name="parameter"> Параметр для проверки. </param>
        /// <returns> Межет/не может быть выполнен (true/false). </returns>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary> Запустить выполнение команды. </summary>
        /// <param name="parameter"> Параметр команды. </param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
