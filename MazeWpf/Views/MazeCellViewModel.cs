namespace MazeWpf.Views
{
    using mazelib;

    public class MazeCellViewModel : ViewModelBase
    {
        public int xPos { get; protected set; }
        public int yPos { get; protected set; }

        public string Coordinates => $"({this.xPos},{this.yPos})";

        public Direction WallDirections { get; protected set; }
        public Direction EscapeDirections { get; protected set; }

        private Maze maze;

        public MazeCellViewModel()
            : base(true)
        {
        }

        public void Configure(Maze maze, int xPos, int yPos)
        {
            base.Configure();
            this.maze = maze;
            this.xPos = xPos;
            this.yPos = yPos;

            this.WallDirections = this.maze.GetWallDirections(xPos, yPos);
            this.EscapeDirections = this.maze.GetEscapeDirections(xPos, yPos);
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
