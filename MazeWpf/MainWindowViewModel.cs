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

            this.MainView = this.viewFactory.CreateViewFromViewModel(await this.viewModelFactory.CreateViewModelAsync<MainMazeViewModel>(vm => {
                vm.Configure(maze);
            }));
        }
    }
}
