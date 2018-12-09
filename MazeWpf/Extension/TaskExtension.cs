namespace MazeWpf.Extension
{
    using System;
    using System.Threading.Tasks;

    public static class TaskExtension
    {
        public static async void TryExecuteAsync(this Task task, Action<Exception> exceptionHandler = null)
        {
            try
            {
                await task;
            }
            catch(Exception ex)
            {
                if (exceptionHandler == null) {
                    throw;
                }

                exceptionHandler(ex);
            }
        }
    }
}
