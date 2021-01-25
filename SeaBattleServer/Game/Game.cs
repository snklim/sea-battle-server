using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattleServer
{
    public class Game
    {
        public int nextPlayer = 1;
        public int status = -1;
        public List<Player> players = new List<Player>();
        Random rnd = new Random();
        public string name { get; set; }

        public GameStatus GameStatus { get; set; }

        public void start()
        {
            this.nextPlayer = (int)Math.Round((double)rnd.Next(2));
            this.status = -1;

            this.players = new List<Player>();

            this.players.Add(new Player());
            this.players.Add(new Player());


            this.players[0].init();
            this.players[1].init();

            if (nextPlayer == 0)
            {
                this.players[1].PlayerStatus = new PlayerAttacking(this.players[1]);
                this.players[0].PlayerStatus = new PlayerDefending(this.players[0]);
            }
            else
            {
                this.players[0].PlayerStatus = new PlayerAttacking(this.players[0]);
                this.players[1].PlayerStatus = new PlayerDefending(this.players[1]);
            }

            GameStatus = new GamePlaying(this);
        }

        public InitData getData(int index)
        {
            var playerOne = this.players[index];
            var playerTwo = this.players[(index + 1) % 2];

            return new InitData
            {
                name = name,
                player = index,
                nextPlayer = this.nextPlayer,
                nextMove = new[]
                {
                    playerOne.generateNextMoveFn(),
                    playerTwo.generateNextMoveFn()
                },
                fields = new[]
                {
                    playerOne.field,
                    playerTwo.field.Select(row => row.Select(cell => new Cell{index = cell.index, type = CellType.Empty, x = cell.x, y = cell.y}).ToList()).ToList()
                }
            };
        }

        public Changes move(Shot shot)
        {
            return GameStatus.Move(shot);
        }
    }
}