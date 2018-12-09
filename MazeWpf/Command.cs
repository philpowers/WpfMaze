namespace MazeWpf
{
    using System;
    using System.Windows.Input;

    public class Command<T> : Command
    {
        public Command(Action<T> execute, Func<bool> canExecute = null) : base(obj => execute((T)obj), canExecute)
        {

        }
    }

    public class Command : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<bool> canExecute;

        public Command(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = obj => execute();
            this.canExecute = canExecute ?? (() => true);
        }

        public Command(Action<object> execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
