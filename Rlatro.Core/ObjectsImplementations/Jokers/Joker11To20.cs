using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.ObjectsImplementations.Jokers
{
    public class SlyJoker : JokerObject
    {
        public SlyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint ChipsBonus = 50;
        private const HandRank TargetHand = HandRank.Pair;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddChips(ChipsBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class WilyJoker : JokerObject
    {
        public WilyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint ChipsBonus = 100;
        private const HandRank TargetHand = HandRank.ThreeOfAKind;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddChips(ChipsBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class CleverJoker : JokerObject
    {
        public CleverJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint ChipsBonus = 80;
        private const HandRank TargetHand = HandRank.TwoPair;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddChips(ChipsBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class DeviousJoker : JokerObject
    {
        public DeviousJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint ChipsBonus = 100;
        private const HandRank TargetHand = HandRank.Straight;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddChips(ChipsBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class CraftyJoker : JokerObject
    {
        public CraftyJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint ChipsBonus = 80;
        private const HandRank TargetHand = HandRank.Flush;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (scoreCtx.HandRank.Contains(TargetHand))
            {
                scoreCtx.AddChips(ChipsBonus);
            }
        }

        public override int BasePrice => 4;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class HalfJoker : JokerObject
    {
        public HalfJoker(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        private const uint MultBonus = 20;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            if (ctx.PlayContainer.Count <= 3)
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override int BasePrice => 5;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class JokerStencil : JokerObject
    {
        public JokerStencil(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            var emptySlots = ctx.JokerContainer.Slots -
                             ctx.JokerContainer.Jokers.Count(j => j is not JokerStencil);
            
            scoreCtx.TimesMult((uint)emptySlots, 1);
        }

        public override int BasePrice => 8;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    // TODO: Utilize OnBuy and OnSell
    public class FourFingers : JokerObject
    {
        public FourFingers(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }
        
        public override int BasePrice => 7;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    public class Mime : JokerObject
    {
        public Mime(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        public override byte AddHeldInHandTriggers(GameContext ctx, CardView cardView)
        {
            return 1;
        }

        public override int BasePrice => 5;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    // TODO: Utilize OnBuy and OnSell and implement logic
    public class CreditCard : JokerObject
    {
        public CreditCard(uint id, Edition edition = Edition.None) : base(id, edition)
        {
        }

        public override int BasePrice => 1;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
}