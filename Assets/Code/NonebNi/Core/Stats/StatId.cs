namespace NonebNi.Core.Stats
{
    public static class StatId
    {
        public const string Health = "health";
        public const string Initiative = "initiative";
        public const string Speed = "speed";
        public const string Focus = "focus";
        public const string Strength = "strength";
        public const string Armor = "armor";
        public const string WeaponRange = "weapon-range";
        public const string Fatigue = "fatigue";
        public const string ActionPoint = "action-point";
        public const string FatigueRecovery = "fatigue-recovery";

        public static readonly string[] All =
        {
            Health,
            Initiative,
            Speed,
            Focus,
            Strength,
            Armor,
            WeaponRange,
            Fatigue,
            ActionPoint,
            FatigueRecovery
        };
    }
}