using System;
using System.Collections.Generic;

namespace SeaBattleServer.Game2.Builders
{
    class FieldBuilder
    {
        private Field field;
        private Random rnd = new Random();

        public FieldBuilder BuildField(int height = 10, int width = 10)
        {
            field = new Field(height, width);

            return this;
        }

        public FieldBuilder BuildShip(int shipLength)
        {
            var attempts = 100;

            while (0 < attempts--)
            {
                var x = rnd.Next(field.Heigth);
                var y = rnd.Next(field.Width);

                var direction = rnd.Next(2);
                var border = new List<Cell>();
                var decks = new List<Cell>();

                if (direction == 0)
                {
                    if (x + shipLength > field.Heigth)
                        x -= x + shipLength - field.Heigth;

                    for (var k = -1; k < 2; k++)
                        for (var i = -1; i < shipLength + 1; i++)
                            if (x + i < field.Heigth && y + k < field.Width && y + k >= 0 && x + i >= 0)
                                if (i >= 0 && i < shipLength && k != 0 || i < 0 || i >= shipLength)
                                    border.Add(new Cell { X = x + i, Y = y + k });

                    for (var i = 0; i < shipLength; i++)
                        decks.Add(new Cell { X = x + i, Y = y, Type = CellType.Deck });
                }
                else
                {

                    if (y + shipLength > field.Width)
                        y -= y + shipLength - field.Width;


                    for (var k = -1; k < 2; k++)
                        for (var i = -1; i < shipLength + 1; i++)
                            if (y + i < field.Width && x + k < field.Heigth && x + k >= 0 && y + i >= 0)
                                if (i >= 0 && i < shipLength && k != 0 || i < 0 || i == shipLength)
                                    border.Add(new Cell { X = x + k, Y = y + i });

                    for (var i = 0; i < shipLength; i++)
                        decks.Add(new Cell { X = x, Y = y + i, Type = CellType.Deck });
                }

                var canPlace = true;

                for (var i = 0; i < decks.Count; i++)
                {
                    var cell = decks[i];
                    if (field.GetState(cell.X, cell.Y) == CellType.Deck)
                    {
                        canPlace = false;
                        break;
                    }
                }

                for (var i = 0; i < border.Count; i++)
                {
                    var cell = border[i];
                    if (field.GetState(cell.X, cell.Y) == CellType.Deck)
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    field.SetShip(decks, border);

                    break;
                }
            }

            return this;
        }

        public Field GetField()
        {
            return field;
        }
    }
}