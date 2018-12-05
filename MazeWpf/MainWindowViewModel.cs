namespace MazeWpf
{
    using System.Threading.Tasks;

    using mazelib;

    using MazeWpf.Interface;
    using MazeWpf.Views;

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IViewModelFactory viewModelFactory;
        private readonly IViewFactory viewFactory;

        private object _mainView;
        public object MainView
        {
            get => this._mainView;
            protected set => this.SetAndRaise(ref this._mainView, value);
        }

        public MainWindowViewModel(IViewModelFactory viewModelFactory, IViewFactory viewFactory) : base(false)
        {
            this.viewModelFactory = viewModelFactory;
            this.viewFactory = viewFactory;
        }

        public override async Task PopulateInitialContentAsync()
        {
            var maze = new Maze(10, 10);

            maze.SetWallsForCell(1, 1, ~Direction.North);
            maze.SetWallsForCell(2, 2, ~Direction.East);
            maze.SetWallsForCell(3, 3, ~Direction.South);
            maze.SetWallsForCell(4, 4, ~Direction.West);

            maze.SetWallsForCell(5, 5, 0);

            this.MainView = this.viewFactory.CreateViewFromViewModel(await this.viewModelFactory.CreateViewModelAsync<MainMazeViewModel>(vm => {
                vm.Configure(maze);
            }));
        }
    }
}
