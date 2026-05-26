using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.ObjectsImplementations.Jokers
{
    [JokerStaticDescription(staticId: 21, JokerRarity.Uncommon, 6, Description = "When Blind is selected, destroy Joker to the right and permanently add double its sell value to its mult")]
    public class CeremonialDagger : JokerObject
    {
        public CeremonialDagger(int staticId, uint runtimeId, Edition edition = Edition.None) : base(staticId, runtimeId, edition)
        {
        }

        private OnBlindSelected OnBlindSelected;

        public override void OnAcquired(GameContext ctx)
        {
            var gameContextClosure = ctx;
            OnBlindSelected = () =>
            {
                // Find the index of this joker in the container
                for (int i = 0; i < gameContextClosure.JokerContainer.Jokers.Count; i++)
                {
                    if (gameContextClosure.JokerContainer.Jokers[i].Id == Id)
                    {
                        if (i < gameContextClosure.JokerContainer.Jokers.Count - 1)
                        {
                            var targetJoker = gameContextClosure.JokerContainer.Jokers[i + 1];
                            var sellValue = (uint)ctx.PersistentState.EconomyHandler.GetJokerSellPrice(targetJoker);
                            Scaling += sellValue * 2;
                            gameContextClosure.JokerContainer.RemoveJoker(gameContextClosure, i + 1);
                        }

                        break;
                    }
                }
            };
            
            ctx.GameEventBus.SubscribeToBlindSelected(OnBlindSelected);
        }
        
        public override void OnRemove(GameContext ctx)
        {
            ctx.GameEventBus.UnsubscribeToBlindSelected(OnBlindSelected);
        }
        
        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            scoreCtx.AddMult(Scaling);
        }

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }

    [JokerStaticDescription(staticId: 22, JokerRarity.Common, 5, Description = "+30 Chips for each remaining discard")]
    public class Banner : JokerObject
    {
        public Banner(int staticId, uint runtimeId, Edition edition = Edition.None) : base(staticId, runtimeId, edition)
        {
        }

        private const uint ChipsPerDiscard = 30;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            var roundState = ctx.GetPhase<RoundState>();
            scoreCtx.AddChips((uint)(roundState.Discards * ChipsPerDiscard));
        }

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }

    [JokerStaticDescription(staticId: 23, JokerRarity.Common, 5, Description = "+15 Mult when 0 discards remaining")]
    public class MysticSummit : JokerObject
    {
        public MysticSummit(int staticId, uint runtimeId, Edition edition = Edition.None) : base(staticId, runtimeId, edition)
        {
        }

        private const uint MultBonus = 15;

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            var roundState = ctx.GetPhase<RoundState>();
            if (roundState.Discards == 0)
            {
                scoreCtx.AddMult(MultBonus);
            }
        }

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }

    [JokerStaticDescription(staticId: 24, JokerRarity.Uncommon, 6, Description = "Adds a Stone Card to your deck when Blind is selected")]
    public class MarbleJoker : JokerObject
    {
        public MarbleJoker(int staticId, uint runtimeId, Edition edition = Edition.None) : base(staticId, runtimeId, edition)
        {
        }

        private OnBlindSelected OnBlindSelected;

        public override void OnAcquired(GameContext ctx)
        {
            var gameContextClosure = ctx;
            OnBlindSelected = () =>
            {
                var stoneCard = CardRegistry.CreateCardWithoutModifiers(ctx, RngActionType.MarbleJokerCard);
                stoneCard.WithEnhancement(Enhancement.Stone);
                gameContextClosure.Deck.Add(stoneCard);
            };

            ctx.GameEventBus.SubscribeToBlindSelected(OnBlindSelected);
        }

        public override void OnRemove(GameContext ctx)
        {
            ctx.GameEventBus.UnsubscribeToBlindSelected(OnBlindSelected);
        }

        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }

    [JokerStaticDescription(staticId: 25, JokerRarity.Uncommon, 5,
        Description = "Adds a Stone Card to your deck when Blind is selected")]
    public class LoyaltyCard : JokerObject
    {
        public LoyaltyCard(int staticId, uint runtimeId, Edition edition = Edition.None) : base(staticId, runtimeId, edition)
        {
            
        }

        public override void OnCardTriggerDone(GameContext ctx, ref ScoreContext scoreCtx)
        {
            Scaling++;
            if (Scaling == 6)
            {
                Scaling = 0;
                scoreCtx.TimesMult(4, 1);
            }
        }
        
        public override bool HasOnPlayedCardTriggerEffect => false;
        public override bool HasOnHeldInHandTriggerEffect => false;
    }
}