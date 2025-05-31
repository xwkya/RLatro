namespace Balatro.Core.CoreRules.Scaling
{
    public static class ScoreRequirement
    {
        private static readonly Dictionary<int, uint> ChipsRequirements = new()
        {
            { 1, 300 },
            { 2, 800 },
            { 3, 2_000 },
            { 4, 5_000 },
            { 5, 11_000 },
            { 6, 20_000 },
            { 7, 35_000 },
            { 8, 55_000 }
        };

        private static float BlindFactor(int round)
        {
            return round switch
            {
                0 => 1f,
                1 => 1.5f,
                2 => 2f,
                _ => 1f
            };
        }

        public static uint GetChipsRequirement(int ante, int round)
        {
            var blindStatus = (round - 1) % 3;
            return (uint)(BlindFactor(round) * ChipsRequirements.GetValueOrDefault<int, uint>(blindStatus, 100));
        }
    }
}