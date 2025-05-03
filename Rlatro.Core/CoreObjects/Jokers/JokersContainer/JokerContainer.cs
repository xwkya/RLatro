using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Jokers.JokersContainer
{
    public class JokerContainer
    {
        public List<JokerObject> Jokers { get; } = new();

        public void RemoveJoker(GameContext ctx, byte jokerIndex)
        {
            Jokers[jokerIndex].OnRemove(ctx);
            Jokers.RemoveAt(jokerIndex);
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
        public bool FourFingers() => false;
        public bool Shortcut() => false;
    }
}