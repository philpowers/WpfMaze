namespace MazeWpf
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using MazeWpf.Interface;
    using MazeWpf.Extension;

    public class AsyncCommand<T> : AsyncCommand
    {
        public AsyncCommand(Func<T, Task> execute, Func<bool> canExecute = null, Action<Exception> exceptionHandler = null)
            : base(obj => execute((T)obj), canExecute, exceptionHandler)
        {
        }
    }

    public class AsyncCommand : IAsyncCommand
    {
        private bool isExecuting;
        private readonly Func<object, Task> execute;
        private readonly Func<bool> canExecute;
        private readonly Action<Exception> exceptionHandler;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            Action<Exception> exceptionHandler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = obj => execute();
            this.canExecute = canExecute;
            this.exceptionHandler = exceptionHandler;
        }

        public AsyncCommand(
            Func<object, Task> execute,
            Func<bool> canExecute = null,
            Action<Exception> exceptionHandler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            this.execute = execute;
            this.canExecute = canExecute;
            this.exceptionHandler = exceptionHandler;
        }

        public bool CanExecute()
        {
            return !isExecuting && (canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (this.CanExecute())
            {
                try
                {
                    isExecuting = true;
                    await execute(parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }
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


        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync(parameter).TryExecuteAsync(exceptionHandler);
        }
    }
}
