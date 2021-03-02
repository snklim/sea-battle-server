using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattleServer
{
    internal static class GameExtensions
    {
        public static Changes GetChanges(this Game2.Game game, Game2.Commands.PlayerMoveCommand cmd)
        {
            var affectedCells = game.Attack(cmd);

            return new Changes
            {
                status = game.Winner,
                valid = affectedCells.Any(),
                nextMove = new Cell { },
                nextPlayer = game.NextPlayer,
                cells = affectedCells.Select(cell => new Cell
                {
                    type = cell.Type == Game2.CellType.Destroyed ? CellType.Killed : CellType.Missed,
                    x = cell.X,
                    y = cell.Y
                }).ToList(),
                x = cmd.Move.X,
                y = cmd.Move.Y
            };
        }

        public static InitData GetInitData(this Game2.Game game, int playerIndex)
        {
            var player1 = game.Player1;
            var player2 = game.Player2;

            if (playerIndex == 1)
            {
                player1 = game.Player2;
                player2 = game.Player1;
            }

            return new InitData
            {
                name = game.Name,
                player = playerIndex,
                nextPlayer = game.NextPlayer,
                nextMove = new Cell[] { },
                fields = new[]
                {
                    GetCells(GetCellTypeSelf(player1)),
                    GetCells(GetCellTypeEnemy(player2))
                }
            };
        }

        private static List<List<Cell>> GetCells(Func<int, int, char> getCellType)
        {
            return Enumerable.Range(0, 10)
                .Select(x => Enumerable.Range(0, 10).Select(y => new Cell
                {
                    x = x,
                    y = y,
                    type = getCellType(x, y)
                })
                .ToList())
                .ToList();
        }

        private static Func<int, int, char> GetCellTypeSelf(Game2.Player player)
        {
            return (x, y) => player.Field.GetState(x, y) switch
            {
                Game2.CellType.Deck => CellType.Deck,
                Game2.CellType.Destroyed => CellType.Killed,
                Game2.CellType.Missed => CellType.Missed,
                _ => CellType.Empty
            };
        }

        private static Func<int, int, char> GetCellTypeEnemy(Game2.Player player)
        {
            return (x, y) => player.Field.GetState(x, y) switch
            {
                Game2.CellType.Destroyed => CellType.Killed,
                Game2.CellType.Missed => CellType.Missed,
                _ => CellType.Empty
            };
        }
    }
}
