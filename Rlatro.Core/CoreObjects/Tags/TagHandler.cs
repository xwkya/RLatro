using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Tags
{
    public class TagHandler
    {
        private List<TagEffect> TagEffectsQueue { get; set; }
        
        public TagHandler()
        {
            TagEffectsQueue = new List<TagEffect>();
        }

        public void GetTag(TagEffect effect, GameContext ctx)
        {
            if (IsEffectImmediate.TryGetValue(effect, out bool isImmediate) && isImmediate)
            {
                var count = GetTagTriggerCount();
                ApplyTagEffect(effect, count, ctx);
            }
            else
            {
                TagEffectsQueue.Add(effect);
            }
        }
        
        private void ApplyTagEffect(TagEffect effect, int count, GameContext ctx)
        {
            switch (effect)
            {
                case TagEffect.BossTag:
                    break; // Boss blinds are not implemented yet
                case TagEffect.StandardTag:
                case TagEffect.BuffoonTag:
                case TagEffect.CharmTag:
                case TagEffect.MeteorTag:
                case TagEffect.EtherealTag:
                    TriggerPackTag(effect, count, ctx);
                    break;
                case TagEffect.GarbageTag:
                    ctx.PersistentState.EconomyHandler.AddGold(count * ctx.PersistentState.UnusedDiscards);
                    break;
                case TagEffect.HandyTag:
                    ctx.PersistentState.EconomyHandler.AddGold(count * ctx.PersistentState.NumberOfHandsPlayed);
                    break;
                case TagEffect.TopUpTag:
                    TriggerTopUpTag(count, ctx);
                    break;
                // TODO: Add the rest ffs
            }
        }
        
        private int GetTagTriggerCount()
        {
            int count = 1;
            while(TagEffectsQueue.Count > 0 && TagEffectsQueue[^1] == TagEffect.DoubleTag)
            {
                count += 1;
                TagEffectsQueue.RemoveAt(TagEffectsQueue.Count - 1);
            }

            return count;
        }

        private void TriggerPackTag(TagEffect effect, int count, GameContext ctx)
        {
            BlindSelectionState selectionState = (BlindSelectionState)ctx.GamePhaseStates[typeof(BlindSelectionState)];
            var packType = effect switch
            {
                TagEffect.StandardTag => BoosterPackType.StandardMega,
                TagEffect.BuffoonTag => BoosterPackType.BuffoonMega,
                TagEffect.CharmTag => BoosterPackType.ArcanaMega,
                TagEffect.MeteorTag => BoosterPackType.CelestialMega,
                TagEffect.EtherealTag => BoosterPackType.SpectralNormal,
                _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
            };
            
            selectionState.RegisterPackOpening(packType, count);
        }

        private void TriggerTopUpTag(int count, GameContext ctx)
        {
            // The tag offers two common jokers, so we add 2*count
            for (int i = 0; i < 2 * count; i++)
            {
                // Apply until we run out of space
                if (ctx.JokerContainer.Slots <= ctx.JokerContainer.Jokers.Count) break;
                
                var randomCommonJoker = ctx.GlobalPoolManager.GenerateJoker(RngActionType.TopUpTagJokerPoll, JokerRarity.Common);
                ctx.JokerContainer.AddJoker(ctx, randomCommonJoker);
            }
        }
        

        private static readonly Dictionary<TagEffect, bool> IsEffectImmediate = new()
        {
            { TagEffect.UncommonTag, false },
            { TagEffect.RareTag, false },
            { TagEffect.NegativeTag, false },
            { TagEffect.FoilTag, false },
            { TagEffect.HolographicTag, false },
            { TagEffect.PolychromeTag, false },
            { TagEffect.InvestmentTag, false },
            { TagEffect.VoucherTag, false },
            { TagEffect.BossTag, true },
            { TagEffect.StandardTag, true },
            { TagEffect.CharmTag, true },
            { TagEffect.MeteorTag, true },
            { TagEffect.BuffoonTag, true },
            { TagEffect.HandyTag, true },
            { TagEffect.GarbageTag, true },
            { TagEffect.EtherealTag, true },
            { TagEffect.CouponTag, false },
            { TagEffect.DoubleTag, true },
            { TagEffect.JuggleTag, false },
            { TagEffect.D6Tag, false },
            { TagEffect.TopUpTag, true },
            { TagEffect.SpeedTag, true },
            { TagEffect.OrbitalTag, true },
            { TagEffect.EconomyTag, true }
        };
    }
}