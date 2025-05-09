using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.ObjectsImplementations.Jokers
{
    public class Joker : JokerObject
    {
        public Joker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
            
        }
        
        private const uint MultBonus = 4;
        public override int BasePrice => 2;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            scoreCtx.AddMult(MultBonus);
        }
    }

    public class GreedyJoker : JokerObject
    {
        public GreedyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
            Suit = OnTriggerSuit;
        }
        
        private const SuitMask OnTriggerSuit = SuitMask.Diamond;
        private const uint OnTriggerMult = 3;

        public override Card64 OnPlayedCardTriggerEffect(GameContext ctx, CardView cardView, Card64 card, ref ScoreContext scoreCtx)
        {
            if ((cardView.Suits & OnTriggerSuit) > 0)
            {
                scoreCtx.AddMult(OnTriggerMult);
            }

            return card;
        }

        public override int BasePrice => 5;
        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class LustyJoker : JokerObject
    {
        public LustyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
            Suit = OnTriggerSuit;
        }
        
        private const SuitMask OnTriggerSuit = SuitMask.Heart;
        private const uint OnTriggerMult = 3;

        public override Card64 OnPlayedCardTriggerEffect(GameContext ctx, CardView cardView, Card64 card, ref ScoreContext scoreCtx)
        {
            if ((cardView.Suits & OnTriggerSuit) > 0)
            {
                scoreCtx.AddMult(OnTriggerMult);
            }

            return card;
        }

        public override int BasePrice => 5;
        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class WrathfulJoker : JokerObject
    {
        public WrathfulJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
            Suit = OnTriggerSuit;
        }
        
        private const SuitMask OnTriggerSuit = SuitMask.Spade;
        private const uint OnTriggerMult = 3;

        public override Card64 OnPlayedCardTriggerEffect(GameContext ctx, CardView cardView, Card64 card, ref ScoreContext scoreCtx)
        {
            if ((cardView.Suits & OnTriggerSuit) > 0)
            {
                scoreCtx.AddMult(OnTriggerMult);
            }

            return card;
        }

        public override int BasePrice => 5;
        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class GluttonousJoker : JokerObject
    {
        public GluttonousJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
            Suit = OnTriggerSuit;
        }
        
        private const SuitMask OnTriggerSuit = SuitMask.Club;
        private const uint OnTriggerMult = 3;

        public override Card64 OnPlayedCardTriggerEffect(GameContext ctx, CardView cardView, Card64 card, ref ScoreContext scoreCtx)
        {
            if ((cardView.Suits & OnTriggerSuit) > 0)
            {
                scoreCtx.AddMult(OnTriggerMult);
            }

            return card;
        }

        public override int BasePrice => 5;
        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }

    public class JollyJoker : JokerObject
    {
        public JollyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint MultBonus = 8;
        private const HandRank TargetHand = HandRank.Pair;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class ZanyJoker : JokerObject
    {
        public ZanyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint MultBonus = 12;
        private const HandRank TargetHand = HandRank.ThreeOfAKind;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class MadJoker : JokerObject
    {
        public MadJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint MultBonus = 10;
        private const HandRank TargetHand = HandRank.TwoPair;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class CrazyJoker : JokerObject
    {
        public CrazyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint MultBonus = 12;
        private const HandRank TargetHand = HandRank.Straight;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class DrollJoker : JokerObject
    {
        public DrollJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint MultBonus = 10;
        private const HandRank TargetHand = HandRank.Flush;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
}