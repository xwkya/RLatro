namespace Balatro.Core.GameEngine.PseudoRng
{
    public class RngController
    {
        private float ProbabilitiesModifier = 1;
        private BalatroRng BalatroRng { get; set; }

        public void Initialize(string seed)
        {
            BalatroRng = new BalatroRng(seed);
        }
        
        public bool ProbabilityCheck(float probability, RngActionType actionType)
        {
            return BalatroRng.NextDouble(actionType) < probability * ProbabilitiesModifier;
        }

        public void SortAndGetShuffle(in Span<int> memoryToShuffle, RngActionType actionType)
        {
            memoryToShuffle.Sort();
            BalatroRng.Shuffle(in memoryToShuffle, actionType);
        }

        public void GetShuffle(in Span<int> memoryToShuffle, RngActionType actionType)
        {
            BalatroRng.Shuffle(in memoryToShuffle, actionType);
        }

        public void GetShuffleContiguous(in Span<int> memoryToShuffle, uint[] keys, RngActionType actionType)
        {
            BalatroRng.ShuffleContiguous(in memoryToShuffle, keys, actionType);
        }
        
        /// <summary>
        /// Gets a random index from the provided indexesToSample, assuming indexesToSample is a contiguous range of indexes.
        /// </summary>
        public int GetRandomIndexContiguous(in Span<int> indexesToSample, uint[] keys, RngActionType actionType)
        {
            return BalatroRng.RandomIndexContiguous(indexesToSample, keys, actionType);
        }
        
        public int GetRandomIndexNonContiguous(in Span<int> indexesToSample, uint[] keys, RngActionType actionType)
        {
            return BalatroRng.RandomIndexNonContiguous(indexesToSample, keys, actionType);
        }

        public int GetRandomElement(in Span<int> elements, RngActionType actionType)
        {
            return BalatroRng.RandomElement(elements, actionType);
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