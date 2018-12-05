namespace mazelib
{
    using System;

    [Flags]
    public enum WallType
    {
        Horizontal = 0x01,
        Vertical   = 0x02
    }

    [Flags]
    public enum Direction
    {
        North  = 0x01,
        South  = 0x02,
        East   = 0x04,
        West   = 0x08,
    }

    public class Maze
    {
        public const Direction AllDirections = Direction.North | Direction.East | Direction.South | Direction.West;

        public uint HorzSize { get; }
        public uint VertSize { get; }

        private readonly WallType[,] WallMap;

        public Maze(uint horzSize, uint vertSize)
        {
            this.HorzSize = horzSize;
            this.VertSize = vertSize;

            this.WallMap = Maze.CreateWallMap(horzSize, vertSize);
        }

        public Direction GetWallDirections(uint xPos, uint yPos)
        {
            if (xPos >= this.HorzSize) {
                throw new ArgumentException(nameof(xPos));
            }
            if (yPos >= this.VertSize) {
                throw new ArgumentException(nameof(yPos));
            }

            Direction wallDirections = 0;

            if ((this.WallMap[xPos, yPos] & WallType.Horizontal) == WallType.Horizontal) {
                wallDirections |= Direction.North;
            }
            if ((this.WallMap[xPos, yPos + 1] & WallType.Horizontal) == WallType.Horizontal) {
                wallDirections |= Direction.South;
            }
            if ((this.WallMap[xPos, yPos] & WallType.Vertical) == WallType.Vertical) {
                wallDirections |= Direction.West;
            }
            if ((this.WallMap[xPos + 1, yPos] & WallType.Vertical) == WallType.Vertical) {
                wallDirections |= Direction.East;
            }

            return wallDirections;
        }

        public Direction GetEscapeDirections(uint xPos, uint yPos)
        {
            var directions = this.GetWallDirections(xPos, yPos);
            directions ^= Maze.AllDirections;
            return directions;
        }

        private static WallType[,] CreateWallMap(uint mazeHorzSize, uint mazeVertSize)
        {
            var wallMap = new WallType[(mazeHorzSize + 1), (mazeVertSize + 1)];

            for (var yPos = 0; yPos <= mazeVertSize; ++yPos) {
                for (var xPos = 0; xPos < mazeHorzSize; ++xPos) {
                    wallMap[xPos, yPos] |= WallType.Horizontal;
                }
            }

            for (var xPos = 0; xPos <= mazeHorzSize; ++xPos) {
                for (var yPos = 0; yPos < mazeVertSize; ++yPos) {
                    wallMap[xPos, yPos] |= WallType.Vertical;
                }
            }

            return wallMap;
        }
    }
}