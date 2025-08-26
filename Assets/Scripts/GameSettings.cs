





using UnityEngine;

namespace ChainEmpires
{
    /// <summary>
    /// Global game settings that control core functionality.
    /// </summary>
    public static class GameSettings
    {
        // Core gameplay settings
        public const bool IsWeb3Enabled = false; // DISABLED FOR ALPHA

        // Networking settings
        public const string ServerAddress = "localhost";
        public const int ServerPort = 8081;

        // Resource management
        public static class Resources
        {
            public const float OreBaseGenerationRate = 5.0f;
            public const float EnergyBaseGenerationRate = 3.0f;
        }

        // Unit settings
        public static class Units
        {
            public const int MaxUnitsPerPlayer = 20;
            public const float MeleeUnitDamage = 10.0f;
            public const float RangedUnitDamage = 8.0f;
        }
    }
}





