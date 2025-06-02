using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.ObjectsImplementations.Jokers;

namespace Balatro.Core.CoreObjects.Jokers.JokersContainer
{
    public class JokerContainer
    {
        /// <summary>
        /// Publicly exposed list of jokers.
        /// </summary>
        public IReadOnlyList<JokerObject> Jokers => JokersArray;
        private List<JokerObject> JokersArray { get; } = new();
        public int Slots { get; set; }
        private int NegativeJokerCount => Jokers.Count(j => j.Edition == Edition.Negative);
        public int AvailableSlots => Slots + NegativeJokerCount - Jokers.Count;
        
        public void RemoveJoker(GameContext ctx, int jokerIndex)
        {
            JokersArray[jokerIndex].OnRemove(ctx);
            JokersArray.RemoveAt(jokerIndex);
        }
        
        public void AddJoker(GameContext ctx, JokerObject joker)
        {
            JokersArray.Add(joker);
            joker.OnAcquired(ctx);
        }
        
        public SuitMask GetJokersSuitChange(Suit suit) => 
            suit switch
            {
                Suit.Spade => SuitMask.Spade,
                Suit.Heart => SuitMask.Heart,
                Suit.Diamond => SuitMask.Diamond,
                Suit.Club => SuitMask.Club,
                _ => SuitMask.None,
            };

        public bool AllFaceCards() => false;
        public bool FourFingers() => Jokers.Any(j => j is FourFingers);
        public bool Shortcut() => false;
        public bool Showman() => false;
    }
}