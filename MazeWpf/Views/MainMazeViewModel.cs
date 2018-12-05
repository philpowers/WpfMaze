namespace MazeWpf.Views
{
    using System.Threading.Tasks;
    using mazelib;

    public class MainMazeViewModel : ViewModelBase
    {
        private Maze maze;

        public MainMazeViewModel()
            : base(true)
        {
        }

        public void Configure(Maze maze)
        {
        }

        public override Task PopulateInitialContentAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class MainMazeViewModelDesigner : MainMazeViewModel
    {
        public MainMazeViewModelDesigner()
        {
        }
    }
}
