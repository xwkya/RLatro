using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.ObjectsImplementations.Consumables
{
    [ConsumableStaticDescription(50, ConsumableType.Planet, HandRank.HighCard)]
    public class Pluto : Consumable
    {
        public Pluto(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.HighCard);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(51, ConsumableType.Planet, HandRank.Pair)]
    public class Mercury : Consumable
    {
        public Mercury(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.Pair);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(52, ConsumableType.Planet, HandRank.TwoPair)]
    public class Uranus : Consumable
    {
        public Uranus(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.TwoPair);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(53, ConsumableType.Planet, HandRank.ThreeOfAKind)]
    public class Venus : Consumable
    {
        public Venus(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.ThreeOfAKind);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(54, ConsumableType.Planet, HandRank.Straight)]
    public class Saturn : Consumable
    {
        public Saturn(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.Straight);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(55, ConsumableType.Planet, HandRank.Flush)]
    public class Jupiter : Consumable
    {
        public Jupiter(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.Flush);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(56, ConsumableType.Planet, HandRank.FullHouse)]
    public class Earth : Consumable
    {
        public Earth(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.FullHouse);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(57, ConsumableType.Planet, HandRank.FourOfAKind)]
    public class Mars : Consumable
    {
        public Mars(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.FourOfAKind);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    [ConsumableStaticDescription(58, ConsumableType.Planet, HandRank.StraightFlush)]
    public class Neptune : Consumable
    {
        public Neptune(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.StraightFlush);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    // Five of a Kind - Planet X (Soft-locked until FiveOfAKind is played)
    [ConsumableStaticDescription(59, ConsumableType.Planet, HandRank.FiveOfAKind, HandRank.FiveOfAKind)]
    public class PlanetX : Consumable
    {
        public PlanetX(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.FiveOfAKind);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    // Flush House - Ceres (Soft-locked until FlushHouse is played)
    [ConsumableStaticDescription(60, ConsumableType.Planet, HandRank.FlushHouse, HandRank.FlushHouse)]
    public class Ceres : Consumable
    {
        public Ceres(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.FlushHouse);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }

    // Flush Five - Eris (Soft-locked until FlushFive is played)
    [ConsumableStaticDescription(61, ConsumableType.Planet, HandRank.FlushFive, HandRank.FlushFive)]
    public class Eris : Consumable
    {
        public Eris(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.PersistentState.HandTracker.UpgradeHand(HandRank.FlushFive);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }
}