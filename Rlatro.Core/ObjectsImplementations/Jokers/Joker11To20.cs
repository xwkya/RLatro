using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.ObjectsImplementations.Jokers
{
    [JokerStaticDescription(staticId: 11, JokerRarity.Common, Description = "If hand played is Pair, +50 Chips.")]
    public class SlyJoker : JokerObject
    {
        public SlyJoker(int staticId, uint runtimeId, Edition edition = Edition.None) : base(staticId, runtimeId, edition)
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

        public override int BasePrice => 3;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 12, JokerRarity.Common, Description = "If hand played is Three of a Kind, +100 Chips.")]
    public class WilyJoker : JokerObject
    {
        public WilyJoker(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
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
    
    [JokerStaticDescription(staticId: 13, JokerRarity.Common, Description = "If hand played is Two Pair, +80 Chips.")]
    public class CleverJoker : JokerObject
    {
        public CleverJoker(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
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
    
    [JokerStaticDescription(staticId: 14, JokerRarity.Common, Description = "If hand played is Straight, +100 Chips.")]
    public class DeviousJoker : JokerObject
    {
        public DeviousJoker(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
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
    
    [JokerStaticDescription(staticId: 15, JokerRarity.Common, Description = "If hand played is Flush, +80 Chips.")]
    public class CraftyJoker : JokerObject
    {
        public CraftyJoker(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
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
    
    [JokerStaticDescription(staticId: 16, JokerRarity.Common, Description = "If 3 or fewer cards are played, +20 Mult.")]
    public class HalfJoker : JokerObject
    {
        public HalfJoker(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
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
    
    [JokerStaticDescription(staticId: 17, JokerRarity.Uncommon, Description = "X1 Mult for each empty Joker slot. (This Joker does not count)")]
    public class JokerStencil : JokerObject
    {
        public JokerStencil(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
        {
        }

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            var emptySlots = ctx.JokerContainer.Slots -
                             ctx.JokerContainer.Jokers.Count(j => j.StaticId != StaticId);
            
            scoreCtx.TimesMult((uint)emptySlots, 1);
        }

        public override int BasePrice => 8;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 18, JokerRarity.Uncommon, Description = "Allows Flushes and Straights to be made with 4 cards.")]
    public class FourFingers : JokerObject
    {
        public FourFingers(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
        {
        }
        
        public override int BasePrice => 7;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
    
    [JokerStaticDescription(staticId: 19, JokerRarity.Uncommon, Description = "Retrigger all card held in hand abilities 1 additional time.")]
    public class Mime : JokerObject
    {
        public Mime(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
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
    
    [JokerStaticDescription(staticId: 20, JokerRarity.Common, Description = "Allows to go up to -20 gold in debt")]
    public class CreditCard : JokerObject
    {
        public CreditCard(int staticId, uint id, Edition edition = Edition.None) : base(staticId, id, edition)
        {
        }

        public override void OnAcquired(GameContext ctx)
        {
            ctx.PersistentState.MinGold = -20;
        }

        public override void OnRemove(GameContext ctx)
        {
            // Check if any other credit card is present
            if (ctx.JokerContainer.Jokers.Count(j => j.StaticId != StaticId) == 1)
            {
                ctx.PersistentState.MinGold = 0;
            }
        }

        public override int BasePrice => 1;
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
}