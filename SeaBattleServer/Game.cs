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

        public void start()
        {
            this.nextPlayer = (int)Math.Round((double)rnd.Next(2));
            this.status = -1;

            this.players = new List<Player>();

            this.players.Add(new Player());
            this.players.Add(new Player());


            this.players[0].init();
            this.players[1].init();
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
                    playerTwo.field
                }
            };
        }

        public Changes move(Shot shot)
        {
            var changes = new Changes
            {
                valid = false,
                x = shot.x,
                y = shot.y,
                cells = new List<Cell>(),
                nextPlayer = this.nextPlayer,
                nextMove = null,
                status = this.status
            };

            var player = this.players[(shot.player + 1) % 2];

            if (this.status == -1 && changes.nextPlayer == shot.player && player.availableMoves.Any(item => item.x == shot.x && item.y == shot.y))
            {
                changes.valid = true;

                var ret = player.Move(shot.x, shot.y, changes.cells);

                changes.nextPlayer = this.nextPlayer = ret.player ? shot.player : (shot.player + 1) % 2;
                changes.nextMove = ret.move;
                changes.status = this.status = ret.status ? shot.player : this.status;
            }

            return changes;
        }
    }
}