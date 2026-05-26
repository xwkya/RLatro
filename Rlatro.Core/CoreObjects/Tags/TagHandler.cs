using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Tags
{
    /// <summary>
    /// Handles acquisition and effects of tags. When a tag is acquired, it is either applied immediately or stored for later use depending on the tag type.
    /// </summary>
    public class TagHandler
    {
        private int[] TagEffects { get; set; }
        private int NumberOfDoubleTags { get; set; }
        
        public TagHandler()
        {
            int numberOfEffects = Enum.GetValues(typeof(TagEffect)).Length;
            TagEffects = new int[numberOfEffects];
            NumberOfDoubleTags = 0;
        }
        
        public void Reset()
        {
            int numberOfEffects = Enum.GetValues(typeof(TagEffect)).Length;
            TagEffects = new int[numberOfEffects];
            NumberOfDoubleTags = 0;
        }
        
        public void AcquireTag(TagEffect effect, GameContext ctx)
        {
            if (effect == TagEffect.DoubleTag)
            {
                NumberOfDoubleTags++;
                return;
            }
            
            if (IsEffectImmediate.TryGetValue(effect, out bool isImmediate) && isImmediate)
            {
                ApplyImmediateTagEffect(effect, NumberOfDoubleTags + 1, ctx);
            }
            else
            {
                TagEffects[(int)effect] += NumberOfDoubleTags + 1;
                NumberOfDoubleTags = 0;
            }
        }
        
        public void RemoveTag(TagEffect effect)
        {
            if (TagEffects[(int)effect] > 0) TagEffects[(int)effect]--;
        }
        
        public int GetTagCount(TagEffect effect)
        {
            return TagEffects[(int)effect];
        }
        
        private void ApplyImmediateTagEffect(TagEffect effect, int count, GameContext ctx)
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

        private void TriggerPackTag(TagEffect effect, int count, GameContext ctx)
        {
            var packType = effect switch
            {
                TagEffect.StandardTag => BoosterPackType.StandardMega,
                TagEffect.BuffoonTag => BoosterPackType.BuffoonMega,
                TagEffect.CharmTag => BoosterPackType.ArcanaMega,
                TagEffect.MeteorTag => BoosterPackType.CelestialMega,
                TagEffect.EtherealTag => BoosterPackType.SpectralNormal,
                _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
            };
            
            var selectionState = ctx.GetPhase<BlindSelectionState>();
            selectionState.RegisterPackOpening(packType, count);
        }

        private void TriggerTopUpTag(int count, GameContext ctx)
        {
            // The tag offers two common jokers, so we add 2*count
            for (int i = 0; i < 2 * count; i++)
            {
                // Apply until we run out of space
                if (ctx.JokerContainer.AvailableSlots <= 0) break;
                
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