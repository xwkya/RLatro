using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreRules.Scoring
{
    public struct ScoreContext
    {
        public uint Chips;
        public uint MultNumerator;
        public uint MultDenominator;
        
        public HandRank HandRank;
        
        public void AddChips(uint chips)
        {
            Chips += chips;
        }

        public void AddMult(uint mult)
        {
            MultNumerator += mult * MultDenominator;
        }
        
        public void TimesMult(uint multNumerator, uint multDenominator)
        {
            MultNumerator *= multNumerator;
            MultDenominator *= multDenominator;
        }
    }
}