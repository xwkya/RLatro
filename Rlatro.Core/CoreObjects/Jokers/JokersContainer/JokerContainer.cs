using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.ObjectsImplementations.Jokers;

namespace Balatro.Core.CoreObjects.Jokers.JokersContainer
{
    public class JokerContainer
    {
        public List<JokerObject> Jokers { get; } = new();
        public int Slots { get; set; }

        public void RemoveJoker(GameContext ctx, int jokerIndex)
        {
            Jokers[jokerIndex].OnRemove(ctx);
            Jokers.RemoveAt(jokerIndex);
        }
        
        public void AddJoker(GameContext ctx, JokerObject joker)
        {
            Jokers.Add(joker);
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