namespace mazelib
{
    using System;
    using System.Collections.Generic;

    public enum InitialMazeConfiguration
    {
        Empty,
        ClosedWalls,
        RandomMaze
    }

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
        public static readonly Direction[] PossibleDirections = { Direction.North, Direction.East, Direction.South, Direction.West };


        public int HorzSize { get; }
        public int VertSize { get; }

        private WallType[,] WallMap;

        public Maze(int horzSize, int vertSize, InitialMazeConfiguration initialMazeConfiguration)
        {
            this.HorzSize = horzSize;
            this.VertSize = vertSize;

            switch (initialMazeConfiguration)
            {
                case InitialMazeConfiguration.Empty:
                    this.WallMap = new WallType[(horzSize + 1), (vertSize + 1)];
                    break;

                case InitialMazeConfiguration.ClosedWalls:
                    this.WallMap = Maze.CreateClosedWallMap(horzSize, vertSize);
                    break;

                case InitialMazeConfiguration.RandomMaze:
                    this.GenerateRandomMaze();
                    break;
            }
        }

        public Direction GetWallDirections(int xPos, int yPos)
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

        public Direction GetEscapeDirections(int xPos, int yPos)
        {
            var directions = this.GetWallDirections(xPos, yPos);
            directions ^= Maze.AllDirections;
            return directions;
        }

        public void SetWallsForCell(int xPos, int yPos, Direction direction)
        {
            if (xPos >= this.HorzSize) {
                throw new ArgumentException(nameof(xPos));
            }
            if (yPos >= this.VertSize) {
                throw new ArgumentException(nameof(yPos));
            }

            WallType xpyp = 0;
            if (direction.HasFlag(Direction.North)) {
                xpyp |= WallType.Horizontal;
            }
            if (direction.HasFlag(Direction.West)) {
                xpyp |= WallType.Vertical;
            }
            this.WallMap[xPos, yPos] = xpyp;

            if (direction.HasFlag(Direction.East)) {
                this.WallMap[xPos + 1, yPos] |= WallType.Vertical;
            } else {
                this.WallMap[xPos + 1, yPos] &= ~WallType.Vertical;
            }

            if (direction.HasFlag(Direction.South)) {
                this.WallMap[xPos, yPos + 1] |= WallType.Horizontal;
            } else {
                this.WallMap[xPos, yPos + 1] &= ~WallType.Horizontal;
            }
        }

        public void RemoveWall(int xPos, int yPos, Direction direction)
        {
            if (xPos >= this.HorzSize) {
                throw new ArgumentException(nameof(xPos));
            }
            if (yPos >= this.VertSize) {
                throw new ArgumentException(nameof(yPos));
            }

            switch(direction) {
                case Direction.North:
                    this.WallMap[xPos, yPos] &= ~WallType.Horizontal;
                    break;
                case Direction.East:
                    this.WallMap[xPos + 1, yPos] &= ~WallType.Vertical;
                    break;
                case Direction.South:
                    this.WallMap[xPos, yPos + 1] &= ~WallType.Horizontal;
                    break;
                case Direction.West:
                    this.WallMap[xPos, yPos] &= ~WallType.Vertical;
                    break;

                default:
                    throw new ArgumentException(nameof(direction));
            }
        }

        public void GenerateRandomMaze()
        {
            this.WallMap = Maze.CreateClosedWallMap(this.HorzSize, this.VertSize);

            var rand = new Random();
            var startX = rand.Next(0, this.HorzSize - 1);
            var startY = rand.Next(0, this.VertSize - 1);

            var visitedMap = new bool[this.HorzSize, this.VertSize];

            var traverseStack = new Stack<(int x, int y)>();

            (int x, int y) currCoords = (startX, startY);

            // Choose a random (starting) direction
            var possibleDirectionStartIdx = rand.Next(PossibleDirections.Length - 1);

            var currDirectionIdx = possibleDirectionStartIdx;
            while(true) {
                var attemptedDirection = PossibleDirections[currDirectionIdx];

                visitedMap[currCoords.x, currCoords.y] = true;

                (int x, int y) coordsNext = currCoords;

                bool canTraverse = true;
                switch(attemptedDirection) {
                    case Direction.North:
                        coordsNext.y -= 1;
                        break;
                    case Direction.East:
                        coordsNext.x += 1;
                        break;
                    case Direction.South:
                        coordsNext.y += 1;
                        break;
                    case Direction.West:
                        coordsNext.x -= 1;
                        break;

                    default:
                        throw new InvalidOperationException($"Detected unsupported direction: {attemptedDirection}");
                }

                if ((coordsNext.x < 0) || (coordsNext.y < 0) || (coordsNext.x >= this.HorzSize) || (coordsNext.y >= this.VertSize) ) {
                    canTraverse = false;
                }

                if (canTraverse) {
                    if (visitedMap[coordsNext.x, coordsNext.y]) {
                        canTraverse = false;
                    }
                }

                if (canTraverse) {
                    // Knock down the wall!

                    // Move in that direction
                    traverseStack.Push(currCoords);
                }


                // Setup to try the next direction
                ++currDirectionIdx;
                if (currDirectionIdx == possibleDirectionStartIdx) {
                    break;
                }
                if (currDirectionIdx >= (PossibleDirections.Length - 1)) {
                    currDirectionIdx = 0;
                }
            }

        }

        // Generates a WallMap that has walls at every possible location
        private static WallType[,] CreateClosedWallMap(int mazeHorzSize, int mazeVertSize)
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