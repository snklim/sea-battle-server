using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattleServer
{
    public class Cell
    {
        public int x { get; set; }
        public int y { get; set; }
        public char type { get; set; }
        public int index { get; set; }
    }

    public class Ship
    {
        public int index { get; set; }
        public List<Cell> ship { get; set; }
        public List<Cell> border { get; set; }
        public int length { get; set; }
        public bool processed { get; set; }
    }

    public class Player
    {
        public List<Cell> availableMoves = new List<Cell>();
        public List<Cell> nextPossibleMoves = new List<Cell>();
        public List<Cell> prevSuccessfullMoves = new List<Cell>();
        public List<List<Cell>> field = new List<List<Cell>>();
        public List<Ship> ships = new List<Ship>();

        int[] shipsTypes = new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

        Random rnd = new Random();

        public void placeShip(int length)
        {

            var max = 10;
            var attempts = 100;

            while (0 < attempts--)
            {
                var x = rnd.Next(10);
                var y = rnd.Next(10);

                var direction = rnd.Next(2);
                var border = new List<Cell>();
                var ship = new List<Cell>();

                if (direction == 0)
                {

                    if (x + length > max)
                        x -= x + length - max;

                    for (var k = -1; k < 2; k++)
                        for (var i = -1; i < length + 1; i++)
                            if (x + i < max && y + k < max && y + k >= 0 && x + i >= 0)
                                if ((i >= 0 && i < length && k != 0) || i < 0 || i >= length)
                                    border.Add(new Cell { x = x + i, y = y + k });

                    for (var i = 0; i < length; i++)
                        ship.Add(new Cell { x = x + i, y = y, type = 'O' });
                }
                else
                {

                    if (y + length > max)
                        y -= y + length - max;


                    for (var k = -1; k < 2; k++)
                        for (var i = -1; i < length + 1; i++)
                            if (y + i < max && x + k < max && x + k >= 0 && y + i >= 0)
                                if ((i >= 0 && i < length && k != 0) || i < 0 || i == length)
                                    border.Add(new Cell { x = x + k, y = y + i });

                    for (var i = 0; i < length; i++)
                        ship.Add(new Cell { x = x, y = y + i, type = 'O' });
                }

                var canPlace = true;

                for (var i = 0; i < ship.Count; i++)
                {
                    var cell = ship[i];
                    if (this.field[cell.x][cell.y].type == 'O')
                    {
                        canPlace = false;
                        break;
                    }
                }

                for (var i = 0; i < border.Count; i++)
                {
                    var cell = border[i];
                    if (this.field[cell.x][cell.y].type == 'O')
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    for (var i = 0; i < border.Count; i++)
                    {
                        var cell = border[i];
                        this.field[cell.x][cell.y] = new Cell { type = ' ', x = cell.x, y = cell.y };
                    }

                    for (var i = 0; i < ship.Count; i++)
                    {
                        var cell = ship[i];
                        this.field[cell.x][cell.y] = new Cell { type = 'O', x = cell.x, y = cell.y, index = this.ships.Count };
                    }

                    this.ships.Add(new Ship
                    {
                        index = this.ships.Count,
                        ship = ship,
                        border = border,
                        length = length
                    });

                    break;
                }
            }
        }

        public void genBotMoves(List<Cell> moves)
        {
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    moves.Add(new Cell { x = i, y = j });
                }
            }
        }

        public bool shotFn(int x, int y, List<Cell> cells)
        {
            var cell = this.field[x][y];

            var shipIndex = cell.type == 'O' ? cell.index : -1;

            if (cell.type == 'O')
            {
                var shipLength = this.ships[shipIndex].length - 1;

                this.ships[shipIndex].length = shipLength;
                this.ships[shipIndex].ship[shipLength].type = 'K';
            }

            cell.type = cell.type == 'O' ? 'K' : 'X';

            cells.Add(cell);

            return cell.type == 'K';
        }

        public void updateAvailableMovesFn()
        {

            this.availableMoves = new List<Cell>();

            for (var i = 0; i < this.field.Count; i++)
            {
                for (var j = 0; j < this.field[i].Count; j++)
                {
                    if (this.field[i][j].type == ' ' || this.field[i][j].type == 'O')
                    {
                        this.availableMoves.Add(new Cell { x = i, y = j });
                    }
                }
            }
        }

        public void drawBorderFn(List<Cell> cells)
        {
            var clearPrevSuccessfullMoves = false;

            for (var i = 0; i < this.ships.Count; i++)
            {
                var ship = this.ships[i];
                if (ship.length == 0 && !ship.processed)
                {
                    for (var j = 0; j < ship.border.Count; j++)
                    {
                        var cell = ship.border[j];
                        this.field[cell.x][cell.y].type = 'B';
                        cells.Add(new Cell { x = cell.x, y = cell.y, type = 'B' });
                        clearPrevSuccessfullMoves = true;
                        ship.processed = true;
                    }
                }
            }

            if (clearPrevSuccessfullMoves)
            {
                this.prevSuccessfullMoves = new List<Cell>();
            }
        }

        public void generateNextMovesFn(int x, int y)
        {

            if (this.field[x][y].type == 'K')
            {

                this.nextPossibleMoves.Add(new Cell { x = x - 1, y = y });

                this.nextPossibleMoves.Add(new Cell { x = x, y = y + 1 });

                this.nextPossibleMoves.Add(new Cell { x = x + 1, y = y });

                this.nextPossibleMoves.Add(new Cell { x = x, y = y - 1 });

                for (var i = 0; i < this.nextPossibleMoves.Count; i++)
                {
                    var nextPossibleMove = this.nextPossibleMoves[i];
                    if (!this.availableMoves.Any(item => item.x == nextPossibleMove.x && item.y == nextPossibleMove.y))
                    {
                        this.nextPossibleMoves.RemoveAt(i);
                        i--;
                    }
                }

                this.prevSuccessfullMoves.Add(new Cell { x = x, y = y });

                if (this.prevSuccessfullMoves.Count > 1)
                {
                    var prevSuccessfullMove = this.prevSuccessfullMoves[this.prevSuccessfullMoves.Count - 1];

                    for (var index = this.prevSuccessfullMoves.Count - 2; index >= 0; index--)
                    {
                        var prevPrevSuccessfullMove = this.prevSuccessfullMoves[index];

                        if ((Math.Abs(prevPrevSuccessfullMove.x - prevSuccessfullMove.x) == 0 && Math.Abs(prevPrevSuccessfullMove.y - prevSuccessfullMove.y) == 1) ||
                            (Math.Abs(prevPrevSuccessfullMove.x - prevSuccessfullMove.x) == 1 && Math.Abs(prevPrevSuccessfullMove.y - prevSuccessfullMove.y) == 0))
                        {

                            if (prevPrevSuccessfullMove.x == prevSuccessfullMove.x)
                            {
                                for (var i = 0; i < this.nextPossibleMoves.Count; i++)
                                {
                                    if (this.nextPossibleMoves[i].x != prevPrevSuccessfullMove.x)
                                    {
                                        this.nextPossibleMoves.RemoveAt(i);
                                        i--;
                                    }
                                }
                            }
                            else
                            {
                                for (var i = 0; i < this.nextPossibleMoves.Count; i++)
                                {
                                    if (this.nextPossibleMoves[i].y != prevPrevSuccessfullMove.y)
                                    {
                                        this.nextPossibleMoves.RemoveAt(i);
                                        i--;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Cell generateNextMoveFn()
        {
            var next = (int)rnd.Next(this.availableMoves.Count);

            var move = this.availableMoves[next];

            if (this.nextPossibleMoves.Count > 0)
            {
                move = this.nextPossibleMoves[(int)rnd.Next(this.nextPossibleMoves.Count)];
            }

            return move;
        }

        public void updateNextMovesFn(int x, int y)
        {
            var nextPossibleMoveIndex = this.nextPossibleMoves.FindIndex(item => item.x == x && item.y == y);
            if (nextPossibleMoveIndex >= 0)
                this.nextPossibleMoves.RemoveAt(nextPossibleMoveIndex);
        }

        public int getAvailableShips()
        {
            var numOfShips = 0;
            for (var i = 0; i < this.ships.Count; i++)
            {
                var ship = this.ships[i];
                if (ship.length > 0)
                    numOfShips++;
            }
            return numOfShips;
        }

        public void init()
        {
            this.availableMoves = new List<Cell>();
            this.nextPossibleMoves = new List<Cell>();
            this.prevSuccessfullMoves = new List<Cell>();
            this.field = new List<List<Cell>>();
            this.ships = new List<Ship>();

            for (var i = 0; i < 10; i++)
            {
                this.field.Add(new List<Cell>());
                for (var j = 0; j < 10; j++)
                {
                    this.field[i].Add(new Cell { type = ' ', x = i, y = j });
                }
            }

            for (var shipTypeIndex = 0; shipTypeIndex < this.shipsTypes.Length; shipTypeIndex++)
            {
                this.placeShip(this.shipsTypes[shipTypeIndex]);
            }

            this.genBotMoves(this.availableMoves);
        }
    }
}
