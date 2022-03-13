using System;

using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.Model
{
    public class NotEnoughMoneyException : Exception
    {
        public uint Money { get; private set; }
        public uint Cost { get; private set; }

        public NotEnoughMoneyException(uint money, uint cost, string? message = null) : base(message) { Money = money; Cost = cost; }
    }


    [Serializable]
    public class InvalidPlacementException : Exception
    {
        public Field Field { get; private set; }

        public InvalidPlacementException(Field field, string? message = null) : base(message) { Field = field; }
    }
}
