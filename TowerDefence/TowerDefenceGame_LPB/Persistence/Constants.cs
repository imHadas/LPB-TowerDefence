namespace TowerDefenceBackend.Persistence
{
    /// <summary>
    /// We could store all constants here, up for debate
    /// </summary>
    public static class Constants
    {
        public const uint CASTLE_STARTING_HEALTH = 20;
        public const uint PLAYER_STARTING_MONEY = 200;

        public const uint BASIC_UNIT_STARTING_HEALTH = 1;
        public const uint BASIC_UNIT_SPEED = 3;
        public const uint BASIC_UNIT_COST = 10;

        public const uint TANK_UNIT_STARTING_HEALTH = 3;
        public const uint TANK_UNIT_SPEED = 1;
        public const uint TANK_UNIT_COST = 50;

        public const uint MAX_TOWER_LEVEL = 3;

        public const uint BASIC_TOWER_COST = 50;
        public const uint SNIPER_TOWER_COST = 100;
        public const uint BOMBER_TOWER_COST = 120;

        public const uint PASSIVE_INCOME = 100;
    }
}
