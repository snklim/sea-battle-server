﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattleServer
{
    public class InitData
    {
        public string name { get; set; }
        public int player { get; set; }
        public int nextPlayer { get; set; }
        public Cell[] nextMove { get; set; }
        public List<List<Cell>>[] fields { get; set; }
    }

    public class Changes
    {
        public bool valid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public List<Cell> cells { get; set; } = new List<Cell>();
        public int nextPlayer { get; set; }
        public Cell nextMove { get; set; }
        public int fieldIndex { get; set; }
        public int status { get; set; }
    }

    public class Shot
    {
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int player { get; set; }
    }

    public class Game
    {
        int nextPlayer = 1;
        public int status = -1;
        List<Player> players = new List<Player>();
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
                changes.nextPlayer = player.shotFn(shot.x, shot.y, changes.cells) ? shot.player : (shot.player + 1) % 2;
                player.updateNextMovesFn(shot.x, shot.y);
                player.drawBorderFn(changes.cells);
                player.updateAvailableMovesFn();
                player.generateNextMovesFn(shot.x, shot.y);
                changes.nextMove = player.generateNextMoveFn();
                this.nextPlayer = changes.nextPlayer;

                if (player.getAvailableShips() == 0)
                    this.status = shot.player;

                changes.status = this.status;
            }

            return changes;
        }
    }
}