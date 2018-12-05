namespace MazeWpf.Views
{
    using mazelib;

    public class MazeCellViewModel : ViewModelBase
    {
        public uint xPos { get; protected set; }
        public uint yPos { get; protected set; }

        public string Coordinates => $"({this.xPos},{this.yPos})";

        private Maze maze;

        public MazeCellViewModel()
            : base(true)
        {
        }

        public void Configure(Maze maze, uint xPos, uint yPos)
        {
            this.maze = maze;
            this.xPos = xPos;
            this.yPos = yPos;
        }
    }

    public class MazeCellViewModelDesigner : MazeCellViewModel
    {
        public MazeCellViewModelDesigner()
        {
            this.xPos = 1;
            this.yPos = 3;
        }
    }
}
