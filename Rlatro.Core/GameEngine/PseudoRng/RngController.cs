namespace Balatro.Core.GameEngine.PseudoRng
{
    public class RngController
    {
        private float Modifier = 1;
        private BalatroRng BalatroRng { get; set; }

        public RngController(string seed)
        {
            BalatroRng = new BalatroRng(seed);
        }

        public bool ProbabilityCheck(float probability, RngActionType actionType)
        {
            return BalatroRng.NextDouble(actionType) < probability * Modifier;
        }

        public void GetShuffle(in Span<int> memoryToShuffle, RngActionType actionType)
        {
            BalatroRng.Shuffle(in memoryToShuffle, actionType);
        }
    }
}