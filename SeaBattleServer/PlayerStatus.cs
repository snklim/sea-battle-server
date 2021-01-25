using System.Collections.Generic;

namespace SeaBattleServer
{
    public abstract class PlayerStatus
    {
        public abstract (bool player, Cell move, bool status, bool valid) Attack(int x, int y, List<Cell> cells);
        public abstract void SwitchStatus();
    }
}