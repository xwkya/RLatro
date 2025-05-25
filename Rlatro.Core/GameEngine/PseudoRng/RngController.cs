namespace Balatro.Core.GameEngine.PseudoRng
{
    public class RngController
    {
        private float ProbabilitiesModifier = 1;
        private BalatroRng BalatroRng { get; set; }

        public RngController(string seed)
        {
            BalatroRng = new BalatroRng(seed);
        }

        public bool ProbabilityCheck(float probability, RngActionType actionType)
        {
            return BalatroRng.NextDouble(actionType) < probability * ProbabilitiesModifier;
        }

        public void GetShuffle(in Span<int> memoryToShuffle, RngActionType actionType)
        {
            BalatroRng.Shuffle(in memoryToShuffle, actionType);
        }

        public void GetShuffle(in Span<int> memoryToShuffle, uint[] keys, RngActionType actionType)
        {
            BalatroRng.Shuffle(in memoryToShuffle, keys, actionType);
        }
        
        /// <summary>
        /// Random number between min and max inclusive
        /// </summary>
        public int RandomInt(int min, int max, RngActionType actionType, string suffix = null)
        {
            return suffix is null ? 
                BalatroRng.NextInt(actionType, min, max) :
                BalatroRng.NextInt(actionType.Key() + suffix, min, max);
        }

        public double GetRandomProbability(RngActionType actionType, string suffix = null)
        {
            return suffix is null ? 
                BalatroRng.NextDouble(actionType) :
                BalatroRng.NextDouble(actionType.Key() + suffix);
        }
    }
}