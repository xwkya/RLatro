using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.ObjectsImplementations.Jokers
{
    [JokerStaticDescription(staticId: 1, JokerRarity.Common, 2, Description = "Adds +4 mult")]
    public class Joker : JokerObject
    {
        public Joker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
        {
        }
        
        private const uint MultBonus = 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            scoreCtx.AddMult(MultBonus);
        }
    }
    
    [JokerStaticDescription(staticId: 2, JokerRarity.Common, 5, Description = "Played Diamonds give +3 Mult.")]
    public class GreedyJoker : JokerObject
    {
        public GreedyJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 3, JokerRarity.Common, 5, Description = "Played Hearts give +3 Mult.")]
    public class LustyJoker : JokerObject
    {
        public LustyJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 4, JokerRarity.Common, 5, Description = "Played Spades give +3 Mult.")]
    public class WrathfulJoker : JokerObject
    {
        public WrathfulJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 5, JokerRarity.Common, 5, Description = "Played Clubs give +3 Mult.")]
    public class GluttonousJoker : JokerObject
    {
        public GluttonousJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => true;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }

    [JokerStaticDescription(staticId: 6, JokerRarity.Common, 4, Description = "If hand played is Pair, +8 Mult.")]
    public class JollyJoker : JokerObject
    {
        public JollyJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 7, JokerRarity.Common, 4, Description = "If hand played is Three of a Kind, +12 Mult.")]
    public class ZanyJoker : JokerObject
    {
        public ZanyJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 8, JokerRarity.Common, 4, Description = "If hand played is Two Pair, +10 Mult.")]
    public class MadJoker : JokerObject
    {
        public MadJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 9, JokerRarity.Common, 4, Description = "If hand played is Straight, +12 Mult.")]
    public class CrazyJoker : JokerObject
    {
        public CrazyJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 10, JokerRarity.Common, 4, Description = "If hand played is Flush, +10 Mult.")]
    public class DrollJoker : JokerObject
    {
        public DrollJoker(int staticId, uint runtimeId, Edition edition = Edition.None) 
            : base(staticId, runtimeId, edition)
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

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
}