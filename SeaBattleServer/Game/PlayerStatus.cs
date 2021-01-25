using System.Collections.Generic;

namespace SeaBattleServer
{
    public abstract class PlayerStatus
    {
        public abstract AttackResponse Attack(int x, int y, List<Cell> cells);
        public abstract void SwitchStatus();
    }
}