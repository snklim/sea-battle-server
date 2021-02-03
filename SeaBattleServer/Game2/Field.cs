using System.Collections.Generic;

namespace SeaBattleServer.Game2
{
    class Field
    {
        private List<List<Cell>> cells = new List<List<Cell>>();

        private List<Ship> ships = new List<Ship>();

        public int Heigth { get; }

        public int Width { get; }

        public int NumberOfShips { get; private set; }

        public Field(int height = 10, int width = 10)
        {
            Heigth = height;
            Width = width;
            for (var i = 0; i < height; i++)
            {
                cells.Add(new List<Cell>());
                for (var j = 0; j < width; j++)
                {
                    cells[i].Add(new Cell { X = i, Y = j, Type = CellType.Empty });
                }
            }
        }

        public void SetShip(List<Cell> decks, List<Cell> border)
        {
            for (var i = 0; i < decks.Count; i++)
            {
                var cell = decks[i];
                this.cells[cell.X][cell.Y] = new Cell { Type = CellType.Deck, X = cell.X, Y = cell.Y, ShipIndex = this.ships.Count };
            }

            this.ships.Add(new Ship
            {
                Decks = decks,
                Border = border,
                ShipLength = decks.Count
            });

            NumberOfShips++;
        }

        public CellType GetState(int x, int y)
        {
            if (x < Heigth && y < Width)
                return cells[x][y].Type;

            return CellType.Empty;
        }

        public bool Attack(Move move, List<Cell> affectedCells)
        {
            if (move.X < Heigth && move.Y < Width)
                return AttackShip(move.X, move.Y, affectedCells);

            return false;
        }

        private bool AttackShip(int x, int y, List<Cell> affectedCells)
        {
            if (cells[x][y].Type == CellType.Deck)
            {
                cells[x][y].Type = CellType.Destroyed;
                var ship = ships[cells[x][y].ShipIndex];
                ship.ShipLength -= 1;

                affectedCells.Add(cells[x][y]);

                if (ship.ShipLength == 0)
                {
                    ship.Border.ForEach(cell =>
                    {
                        cell.Type = CellType.Missed;
                        cells[cell.X][cell.Y].Type = CellType.Missed;
                        affectedCells.Add(cell);
                    });
                    NumberOfShips--;
                }

                return true;
            }
            else
            {
                cells[x][y].Type = CellType.Missed;
                affectedCells.Add(cells[x][y]);
            }

            return false;
        }
    }
}