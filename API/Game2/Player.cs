using SeaBattleServer.Game2.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattleServer.Game2
{
    class Player
    {
        private Random rnd = new Random();
        private List<Move> nextPossibleMoves = new List<Move>();
        private List<Move> prevSuccessfullMoves = new List<Move>();
        private List<Move> availableMoves = Enumerable.Range(0, 10)
            .SelectMany(x => Enumerable.Range(0, 10)
            .Select(y => new Move { X = x, Y = y }))
            .ToList();

        public Guid PlayerId { get; } = Guid.NewGuid();

        public Field Field { get; set; }

        public PlayerState State { get; set; }

        public Move NextMove()
        {
            if (!availableMoves.Any()) throw new Exception("There aren't any move to go");

            var next = (int)rnd.Next(availableMoves.Count);

            var move = availableMoves[next];

            while (this.nextPossibleMoves.Count > 0)
            {
                var move1 = this.nextPossibleMoves[(int)rnd.Next(this.nextPossibleMoves.Count)];

                if (availableMoves.Any(availableMove => availableMove.X == move1.X && availableMove.Y == move1.Y))
                    return move1;
                else
                {
                    this.nextPossibleMoves.Remove(move1);
                }
            }

            return move;
        }

        public void Attack(Move move, List<Cell> affectedCells)
        {
            if (availableMoves.Any(availableMove => availableMove.X == move.X && availableMove.Y == move.Y))
            {
                State.Attack(move, affectedCells);

                affectedCells.ForEach(cell =>
                {
                    if (availableMoves.Any(availableMove => availableMove.X == cell.X && availableMove.Y == cell.Y))
                        availableMoves.Remove(availableMoves.First(availableMove => availableMove.X == cell.X && availableMove.Y == cell.Y));
                });

                if (affectedCells.Any(cell => cell.Type == CellType.Destroyed))
                    if (affectedCells.Count > 1)
                    {
                        prevSuccessfullMoves = new List<Move>();
                        nextPossibleMoves = new List<Move>();
                    }
                    else if (affectedCells.Count == 1)
                    {
                        GenerateNextMoves(move);
                    }
            }
        }

        private void GenerateNextMoves(Move move)
        {
            var x = move.X;
            var y = move.Y;

            this.nextPossibleMoves.Add(new Move { X = x - 1, Y = y });

            this.nextPossibleMoves.Add(new Move { X = x, Y = y + 1 });

            this.nextPossibleMoves.Add(new Move { X = x + 1, Y = y });

            this.nextPossibleMoves.Add(new Move { X = x, Y = y - 1 });

            for (var i = 0; i < this.nextPossibleMoves.Count; i++)
            {
                var nextPossibleMove = this.nextPossibleMoves[i];
                if (!this.availableMoves.Any(item => item.X == nextPossibleMove.X && item.Y == nextPossibleMove.Y))
                {
                    this.nextPossibleMoves.RemoveAt(i);
                    i--;
                }
            }

            this.prevSuccessfullMoves.Add(new Move { X = x, Y = y });

            if (this.prevSuccessfullMoves.Count > 1)
            {
                var prevSuccessfullMove = this.prevSuccessfullMoves[this.prevSuccessfullMoves.Count - 1];

                for (var index = this.prevSuccessfullMoves.Count - 2; index >= 0; index--)
                {
                    var prevPrevSuccessfullMove = this.prevSuccessfullMoves[index];

                    if ((Math.Abs(prevPrevSuccessfullMove.X - prevSuccessfullMove.X) == 0 && Math.Abs(prevPrevSuccessfullMove.Y - prevSuccessfullMove.Y) == 1) ||
                        (Math.Abs(prevPrevSuccessfullMove.X - prevSuccessfullMove.X) == 1 && Math.Abs(prevPrevSuccessfullMove.Y - prevSuccessfullMove.Y) == 0))
                    {

                        if (prevPrevSuccessfullMove.X == prevSuccessfullMove.X)
                        {
                            for (var i = 0; i < this.nextPossibleMoves.Count; i++)
                            {
                                if (this.nextPossibleMoves[i].X != prevPrevSuccessfullMove.X)
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
                                if (this.nextPossibleMoves[i].Y != prevPrevSuccessfullMove.Y)
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
}