namespace MazeWpf.Views
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using mazelib;

    using MazeWpf.Interface;

    public class MainMazeViewModel : ViewModelBase
    {
        private ObservableCollection<ObservableCollection<object>> _mazeRows;
        public ObservableCollection<ObservableCollection<object>> MazeRows
        {
            get => this._mazeRows;
            protected set => this.SetAndRaise(ref this._mazeRows, value);
        }

        private readonly IViewModelFactory viewModelFactory;
        private readonly IViewFactory viewFactory;

        private Maze maze;

        public MainMazeViewModel(IViewModelFactory viewModelFactory, IViewFactory viewFactory)
            : base(true)
        {
            this.viewModelFactory = viewModelFactory;
            this.viewFactory = viewFactory;
        }

        public void Configure(Maze maze)
        {
            base.Configure();
            this.maze = maze;
        }

        public override async Task PopulateInitialContentAsync()
        {
            var rows = new ObservableCollection<ObservableCollection<object>>();
            for (uint y = 0; y < this.maze.VertSize; ++y) {
                var row = new ObservableCollection<object>();

                for (uint x = 0; x < this.maze.HorzSize; ++x) {
                    row.Add(this.viewFactory.CreateViewFromViewModel(await this.viewModelFactory.CreateViewModelAsync<MazeCellViewModel>(vm => {
                        vm.Configure(this.maze, x, y);
                    })));
                }

                rows.Add(row);
            }

            this.MazeRows = rows;
        }
    }

    public class MainMazeViewModelDesigner : MainMazeViewModel
    {
        public MainMazeViewModelDesigner() : base(null, null)
        {
        }
    }
}
