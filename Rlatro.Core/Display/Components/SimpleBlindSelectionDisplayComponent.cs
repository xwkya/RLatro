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
                CoreObjects.Tags.TagEffect.UncommonTag => "Next shop will have a free Uncommon Joker",
                CoreObjects.Tags.TagEffect.RareTag => "Next shop will have a free Rare Joker",
                CoreObjects.Tags.TagEffect.NegativeTag => "The next base edition Joker you find in a Shop becomes negative and free",
                CoreObjects.Tags.TagEffect.FoilTag => "The next base edition Joker you find in a Shop becomes foil and free", 
                CoreObjects.Tags.TagEffect.HolographicTag => "The next base edition Joker you find in a Shop becomes holographic and free",
                CoreObjects.Tags.TagEffect.PolychromeTag => "The next base edition Joker you find in a Shop becomes polychrome and free",
                CoreObjects.Tags.TagEffect.InvestmentTag => "Gain $25 after defeating the next Boss Blind",
                CoreObjects.Tags.TagEffect.VoucherTag => "Adds a voucher to the shop",
                CoreObjects.Tags.TagEffect.BossTag => "Rerolls the boss blind",
                CoreObjects.Tags.TagEffect.StandardTag => "Opens a free Mega Standard Pack",
                CoreObjects.Tags.TagEffect.CharmTag => "Opens a free Mega Arcana Pack", 
                CoreObjects.Tags.TagEffect.MeteorTag => "Opens a free Mega Celestial Pack",
                CoreObjects.Tags.TagEffect.BuffoonTag => "Opens a free Mega Buffoon Pack",
                CoreObjects.Tags.TagEffect.HandyTag => $"Earn ${gameContext.PersistentState.NumberOfHandsPlayed} (1$ per hand played this ante)",
                CoreObjects.Tags.TagEffect.GarbageTag => $"Earn ${gameContext.PersistentState.UnusedDiscards} (1$ per unused discard this ante)",
                CoreObjects.Tags.TagEffect.EtherealTag => "Opens a free Spectral Pack",
                CoreObjects.Tags.TagEffect.CouponTag => "In the next shop, initial Jokers, Consumables Cards and Booster Packs are free",
                CoreObjects.Tags.TagEffect.DoubleTag => "Doubles the effect of the next tag",
                CoreObjects.Tags.TagEffect.JuggleTag => "+3 Hand size for the next round only",
                CoreObjects.Tags.TagEffect.D6Tag => "In the next shop, rerolls start at $0",
                CoreObjects.Tags.TagEffect.TopUpTag => "Create up to 2 Common Jokers",
                CoreObjects.Tags.TagEffect.SpeedTag => "Gives $5 for each Blind you've skipped this run (Guaranteed $5 as it includes this blind skip)",
                CoreObjects.Tags.TagEffect.OrbitalTag => "Upgrades a specified random Poker Hand by three levels",
                CoreObjects.Tags.TagEffect.EconomyTag => "Double your money (max $40)",
                _ => "Unknown effect"
            };
        }
    }
}