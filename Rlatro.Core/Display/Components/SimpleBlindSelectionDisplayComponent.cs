using System.Text;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.Display.Components
{
    public static class SimpleBlindSelectionDisplayComponent
    {
        public static void DisplayBlindSelectionState(StringBuilder sb, GameContext gameContext, BlindSelectionState blindSelectionState)
        {
            sb.AppendLine("=== BLIND SELECTION ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()} | Round: {gameContext.PersistentState.Round} | Ante: {gameContext.PersistentState.Ante}");
            sb.AppendLine();

            DisplayCurrentBlind(sb, gameContext);
            DisplaySkipTags(sb, blindSelectionState, gameContext);
            
            sb.AppendLine();
            sb.AppendLine("Commands: 'p' (play blind), 's' (skip blind - gain tag), 'sj [index]' (sell joker), 'sc [index]' (sell consumable), 'uc [index] [targets]' (use consumable)");
            sb.AppendLine();
        }

        private static void DisplayCurrentBlind(StringBuilder sb, GameContext gameContext)
        {
            var roundType = (gameContext.PersistentState.Round - 1) % 3;
            var blindName = roundType switch
            {
                0 => "Small Blind",
                1 => "Big Blind", 
                2 => "Boss Blind",
                _ => "Unknown Blind"
            };

            var blindReward = gameContext.PersistentState.EconomyHandler.CalculateRoundGold();
            
            sb.AppendLine($"Current Blind: {blindName} (Reward: ${blindReward})");
            
            // Show if this is a boss blind (can't be skipped)
            if (roundType == 2)
            {
                sb.AppendLine("⚠️  Boss blinds cannot be skipped!");
            }
        }

        private static void DisplaySkipTags(StringBuilder sb, BlindSelectionState blindSelectionState, GameContext gameContext)
        {
            var roundType = (gameContext.PersistentState.Round - 1) % 3;
            
            // Boss blinds cannot be skipped
            if (roundType == 2)
            {
                sb.AppendLine("Skip Options: None (Boss Blind)");
                return;
            }

            // Show available skip tag
            var tagIndex = roundType; // 0 for small blind, 1 for big blind
            if (tagIndex < blindSelectionState.AnteTags.Length)
            {
                var skipTag = blindSelectionState.AnteTags[tagIndex];
                sb.AppendLine($"Skip Reward: {GetTagDisplayName(skipTag)} Tag");
                sb.AppendLine($"  └─ {GetTagDescription(skipTag, gameContext)}");
            }
            else
            {
                sb.AppendLine("Skip Options: Available (tag not yet generated)");
            }
        }

        private static string GetTagDisplayName(CoreObjects.Tags.TagEffect tagEffect)
        {
            return tagEffect switch
            {
                CoreObjects.Tags.TagEffect.UncommonTag => "Uncommon",
                CoreObjects.Tags.TagEffect.RareTag => "Rare", 
                CoreObjects.Tags.TagEffect.NegativeTag => "Negative",
                CoreObjects.Tags.TagEffect.FoilTag => "Foil",
                CoreObjects.Tags.TagEffect.HolographicTag => "Holographic",
                CoreObjects.Tags.TagEffect.PolychromeTag => "Polychrome",
                CoreObjects.Tags.TagEffect.InvestmentTag => "Investment",
                CoreObjects.Tags.TagEffect.VoucherTag => "Voucher",
                CoreObjects.Tags.TagEffect.BossTag => "Boss",
                CoreObjects.Tags.TagEffect.StandardTag => "Standard",
                CoreObjects.Tags.TagEffect.CharmTag => "Charm",
                CoreObjects.Tags.TagEffect.MeteorTag => "Meteor",
                CoreObjects.Tags.TagEffect.BuffoonTag => "Buffoon",
                CoreObjects.Tags.TagEffect.HandyTag => "Handy",
                CoreObjects.Tags.TagEffect.GarbageTag => "Garbage",
                CoreObjects.Tags.TagEffect.EtherealTag => "Ethereal",
                CoreObjects.Tags.TagEffect.CouponTag => "Coupon",
                CoreObjects.Tags.TagEffect.DoubleTag => "Double",
                CoreObjects.Tags.TagEffect.JuggleTag => "Juggle",
                CoreObjects.Tags.TagEffect.D6Tag => "D6",
                CoreObjects.Tags.TagEffect.TopUpTag => "Top Up",
                CoreObjects.Tags.TagEffect.SpeedTag => "Speed",
                CoreObjects.Tags.TagEffect.OrbitalTag => "Orbital",
                CoreObjects.Tags.TagEffect.EconomyTag => "Economy",
                _ => tagEffect.ToString()
            };
        }

        private static string GetTagDescription(CoreObjects.Tags.TagEffect tagEffect, GameContext gameContext)
        {
            return tagEffect switch
            {
                CoreObjects.Tags.TagEffect.UncommonTag => "Next shop has more Uncommon Jokers",
                CoreObjects.Tags.TagEffect.RareTag => "Next shop has more Rare Jokers",
                CoreObjects.Tags.TagEffect.NegativeTag => "Next shop Joker has Negative edition",
                CoreObjects.Tags.TagEffect.FoilTag => "Next shop Joker has Foil edition", 
                CoreObjects.Tags.TagEffect.HolographicTag => "Next shop Joker has Holographic edition",
                CoreObjects.Tags.TagEffect.PolychromeTag => "Next shop Joker has Polychrome edition",
                CoreObjects.Tags.TagEffect.InvestmentTag => "Earn interest on your money",
                CoreObjects.Tags.TagEffect.VoucherTag => "Adds vouchers to the shop",
                CoreObjects.Tags.TagEffect.BossTag => "Rerolls the boss blind",
                CoreObjects.Tags.TagEffect.StandardTag => "Opens a Mega Standard Pack",
                CoreObjects.Tags.TagEffect.CharmTag => "Opens a Mega Arcana Pack", 
                CoreObjects.Tags.TagEffect.MeteorTag => "Opens a Mega Celestial Pack",
                CoreObjects.Tags.TagEffect.BuffoonTag => "Opens a Mega Buffoon Pack",
                CoreObjects.Tags.TagEffect.HandyTag => $"Earn ${gameContext.PersistentState.NumberOfHandsPlayed} (1$ per hand played this ante)",
                CoreObjects.Tags.TagEffect.GarbageTag => $"Earn ${gameContext.PersistentState.UnusedDiscards} (1$ per unused discard this ante)",
                CoreObjects.Tags.TagEffect.EtherealTag => "Opens a Spectral Pack",
                CoreObjects.Tags.TagEffect.CouponTag => "Free reroll in next shop",
                CoreObjects.Tags.TagEffect.DoubleTag => "Doubles the effect of the next tag",
                CoreObjects.Tags.TagEffect.JuggleTag => "Create a copy of the last played poker hand",
                CoreObjects.Tags.TagEffect.D6Tag => "Reroll all jokers in the shop",
                CoreObjects.Tags.TagEffect.TopUpTag => "Create up to 2 Common Jokers",
                CoreObjects.Tags.TagEffect.SpeedTag => "Skip to the Big Blind",
                CoreObjects.Tags.TagEffect.OrbitalTag => "Upgrade the most played poker hand by 3 levels",
                CoreObjects.Tags.TagEffect.EconomyTag => "Double your money (max $50)",
                _ => "Unknown effect"
            };
        }
    }
}