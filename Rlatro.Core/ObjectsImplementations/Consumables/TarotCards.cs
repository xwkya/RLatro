using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.ObjectsImplementations.Consumables
{
    [ConsumableStaticDescription(1, ConsumableType.Tarot)]
    public class TheFool : Consumable
    {
        public TheFool(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            var consumable = context.CoreObjectsFactory
                .CreateConsumable(context.PersistentState.TheFoolStorageStaticId!.Value);

            context.ConsumableContainer.AddConsumable(consumable);
            context.GameEventBus.PublishConsumableAddedToContext(consumable.StaticId);
            context.PersistentState.TheFoolStorageStaticId = null;
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.PersistentState.TheFoolStorageStaticId is not null;
        }
    }

    [ConsumableStaticDescription(2, ConsumableType.Tarot)]
    public class TheMagician : Consumable
    {
        public TheMagician(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        static Card64 MakeLucky(Card64 card) => card.WithEnhancement(Enhancement.Lucky);

        public override void Apply(GameContext context, int[] targetCards)
        {
            for (int i = 0; i < targetCards.Length; i++)
            {
                context.Hand.TransformCard(MakeLucky, targetCards[i]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Magician can be used on at most two cards
            return ctx.Hand.Count >= targetCards.Length
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length > 0
                   && targetCards.Length < 2;
        }
    }

    [ConsumableStaticDescription(3, ConsumableType.Tarot)]
    public class TheHighPriestess : Consumable
    {
        public TheHighPriestess(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        public override void Apply(GameContext context, int[] targetCards)
        {
            var consumable = context.GlobalPoolManager
                .GenerateConsumable(RngActionType.TheHighPriestess, ConsumableType.Planet);
            context.ConsumableContainer.AddConsumable(consumable);
            
            var availableSpace = context.ConsumableContainer.Capacity - context.ConsumableContainer.Consumables.Count;
            if (availableSpace < 1)
            {
                return;
            }
            
            var generateConsumable = context.GlobalPoolManager
                .GenerateConsumable(RngActionType.TheHighPriestess, ConsumableType.Planet);
            context.ConsumableContainer.AddConsumable(generateConsumable);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.ConsumableContainer.Consumables.Count < ctx.ConsumableContainer.Capacity;
        }
    }

    [ConsumableStaticDescription(4, ConsumableType.Tarot)]
    public class TheEmpress : Consumable
    {
        public TheEmpress(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 MakeMult(Card64 card) => card.WithEnhancement(Enhancement.Mult);

        public override void Apply(GameContext context, int[] targetCards)
        {
            for (int i = 0; i < targetCards.Length; i++)
            {
                context.Hand.TransformCard(MakeMult, targetCards[i]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Empress can be used on at most two cards
            return ctx.Hand.Count >= targetCards.Length
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length > 0
                   && targetCards.Length < 2;
        }
    }
    
    [ConsumableStaticDescription(5, ConsumableType.Tarot)]
    public class TheEmperor : Consumable
    {
        public TheEmperor(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            var consumable = context.GlobalPoolManager
                .GenerateConsumable(RngActionType.TheEmperor, ConsumableType.Tarot);
            context.ConsumableContainer.AddConsumable(consumable);
            
            var availableSpace = context.ConsumableContainer.Capacity - context.ConsumableContainer.Consumables.Count;
            if (availableSpace < 1)
            {
                return;
            }
            
            var generateConsumable = context.GlobalPoolManager
                .GenerateConsumable(RngActionType.TheEmperor, ConsumableType.Tarot);
            context.ConsumableContainer.AddConsumable(generateConsumable);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.ConsumableContainer.Consumables.Count < ctx.ConsumableContainer.Capacity;
        }
    }
    
    [ConsumableStaticDescription(6, ConsumableType.Tarot)]
    public class TheHierophant : Consumable
    {
        public TheHierophant(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 MakeBonus(Card64 card) => card.WithEnhancement(Enhancement.Bonus);

        public override void Apply(GameContext context, int[] targetCards)
        {
            for (int i = 0; i < targetCards.Length; i++)
            {
                context.Hand.TransformCard(MakeBonus, targetCards[i]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Hierophant can be used on at most two cards
            return ctx.Hand.Count >= targetCards.Length
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length > 0
                   && targetCards.Length < 2;
        }
    }
    
    [ConsumableStaticDescription(7, ConsumableType.Tarot)]
    public class TheLovers : Consumable
    {
        public TheLovers(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 MakeWild(Card64 card) => card.WithEnhancement(Enhancement.Wild);

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.Hand.TransformCard(MakeWild, targetCards[0]);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Lovers can be used on exactly 1 card
            return ctx.Hand.Count > 0
                   && targetCards.Length == 1
                   && targetCards[1] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(8, ConsumableType.Tarot)]
    public class TheChariot : Consumable
    {
        public TheChariot(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 MakeSteel(Card64 card) => card.WithEnhancement(Enhancement.Steel);

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.Hand.TransformCard(MakeSteel, targetCards[0]);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Chariot can be used on exactly 1 card
            return ctx.Hand.Count > 0
                   && targetCards.Length == 1
                   && targetCards[1] < ctx.Hand.Count;
        }
    }
    
    [ConsumableStaticDescription(9, ConsumableType.Tarot)]
    public class Justice : Consumable
    {
        public Justice(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 MakeGlass(Card64 card) => card.WithEnhancement(Enhancement.Glass);

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.Hand.TransformCard(MakeGlass, targetCards[0]);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // Judgement can be used on exactly 1 card
            return ctx.Hand.Count > 0
                   && targetCards.Length == 1
                   && targetCards[1] < ctx.Hand.Count;
        }
    }
    
    [ConsumableStaticDescription(10, ConsumableType.Tarot)]
    public class TheHermit : Consumable
    {
        private const int MaxGrantedGold = 20;
        public TheHermit(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        public override void Apply(GameContext context, int[] targetCards)
        {
            if (context.PersistentState.EconomyHandler.GetCurrentGold() <= 0) return;
            var goldToAdd = int.Min(context.PersistentState.EconomyHandler.GetCurrentGold(), MaxGrantedGold);
            context.PersistentState.EconomyHandler.AddGold(goldToAdd);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }
    
    [ConsumableStaticDescription(11, ConsumableType.Tarot)]
    public class WheelOfFortune : Consumable
    {
        public WheelOfFortune(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        private Edition GetEdition(RngController rngController)
        {
            // 50% foil, 12.5% Holo, 3.75% Poly
            var sample = rngController.RandomInt(1, 100, RngActionType.WheelOfFortune); // Balatro source code uses the same action type for edition
            if (sample <= 50) return Edition.Foil;
            if (sample <= 85) return Edition.Holo;
            return Edition.Poly;
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            var sample = context.RngController.RandomInt(0, 3, RngActionType.WheelOfFortune);
            if (sample != 0) return;
            
            var edition = GetEdition(context.RngController);
            
            var jokerIndexToModify = GetRandomEligibleJokerIndex(context);
            context.JokerContainer.Jokers[jokerIndexToModify].Edition = edition;
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.JokerContainer.Jokers.Any(j => j.Edition == Edition.None);
        }
        
        /// <summary>
        /// Performs a shuffle of the jokers based on their keys.
        /// This is necessary to ensure that the joker selection is not influenced by their placement.
        /// This trick is commonly used in Balatro's source code.
        /// </summary>
        private int GetRandomEligibleJokerIndex(GameContext context)
        {
            // Count how many jokers are elligible
            int eligibleCount = 0;
            foreach (var joker in context.JokerContainer.Jokers)
            {
                if (joker.Edition == Edition.None)
                {
                    eligibleCount++;
                }
            }
            
            Span<int> eligibleJokerIndexes = stackalloc int[eligibleCount];
            uint[] eligibleJokerKeys = new uint[eligibleCount];

            eligibleCount = 0;
            for (int i = 0; i < context.JokerContainer.Jokers.Count; i++)
            {
                var joker = context.JokerContainer.Jokers[i];
                if (joker.Edition == Edition.None)
                {
                    eligibleJokerIndexes[eligibleCount] = i;
                    eligibleJokerKeys[eligibleCount] = joker.Id;
                    eligibleCount++;
                }
            }
            
            // The indexes can be 2, 4 for example. Hence a contiguous sample is not possible
            context.RngController.GetRandomIndexNonContiguous(eligibleJokerIndexes, eligibleJokerKeys, RngActionType.WheelOfFortune);
            return eligibleJokerIndexes[0];
        }
    }

    [ConsumableStaticDescription(12, ConsumableType.Tarot)]
    public class Strength : Consumable
    {
        public Strength(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 IncreaseRank(Card64 card) => card.WithRank((Rank)((int)(card.GetRank() + 1) % 13));

        public override void Apply(GameContext context, int[] targetCards)
        {
            for (int i = 0; i < targetCards.Length; i++)
            {
                context.Hand.TransformCard(IncreaseRank, targetCards[i]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // Strength can be used on at most two cards
            return ctx.Hand.Count >= targetCards.Length
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length > 0
                   && targetCards.Length < 2;
        }
    }

    [ConsumableStaticDescription(13, ConsumableType.Tarot)]
    public class TheHangedMan : Consumable
    {
        public TheHangedMan(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        public override void Apply(GameContext context, int[] targetCards)
        {
            context.Hand.Remove(targetCards);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Hanged Man can be used on at most two cards
            return ctx.Hand.Count >= targetCards.Length
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length > 0
                   && targetCards.Length < 2;
        }
    }
    
    [ConsumableStaticDescription(14, ConsumableType.Tarot)]
    public class Death : Consumable
    {
        public Death(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        Card64 TransformCardInto(Card64 card, Card64 targetCard)
        {
            return Card64.Create(raw: targetCard.GetRaw(), id: card.Id).WithChipsUpgrade(targetCard.GetChipsUpgrade());
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Transform lower index into higher index
            if (targetCards[0] > targetCards[1])
            {
                // 1 -> 0
                context.Hand.TransformCard(c => TransformCardInto(c, context.Hand.Span[targetCards[1]]), targetCards[0]);
            }
            else
            {
                // 0 -> 1
                context.Hand.TransformCard(c => TransformCardInto(c, context.Hand.Span[targetCards[0]]), targetCards[1]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // Death can be used on exactly two cards
            return ctx.Hand.Count >= 2
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length == 2;
        }
    }
    
    [ConsumableStaticDescription(15, ConsumableType.Tarot)]
    public class Temperance : Consumable
    {
        private const int MaxGrantedGold = 50;
        public Temperance(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        public override void Apply(GameContext context, int[] targetCards)
        {
            var goldToAdd = 0;
            foreach (var joker in context.JokerContainer.Jokers)
            {
                goldToAdd += context.PersistentState.EconomyHandler.GetJokerSellPrice(joker);
            }
            
            goldToAdd = int.Min(goldToAdd, MaxGrantedGold);
            context.PersistentState.EconomyHandler.AddGold(goldToAdd);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }
    
    [ConsumableStaticDescription(16, ConsumableType.Tarot)]
    public class TheDevil : Consumable
    {
        public TheDevil(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        static Card64 MakeGold(Card64 card) => card.WithEnhancement(Enhancement.Gold);

        public override void Apply(GameContext context, int[] targetCards)
        {
            context.Hand.TransformCard(MakeGold, targetCards[0]);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Devil can be used on exactly 1 card
            return ctx.Hand.Count > 0
                   && targetCards.Length == 1
                   && targetCards[1] < ctx.Hand.Count;
        }
    }
    
    [ConsumableStaticDescription(17, ConsumableType.Tarot)]
    public class TheTower : Consumable
    {
        public TheTower(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        static Card64 MakeStone(Card64 card) => card.WithEnhancement(Enhancement.Stone);
        
        public override void Apply(GameContext context, int[] targetCards)
        {
            context.Hand.TransformCard(MakeStone, targetCards[0]);
        }
        
        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The Tower can be used on exactly 1 card
            return ctx.Hand.Count > 0
                   && targetCards.Length == 1
                   && targetCards[1] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(18, ConsumableType.Tarot)]
    public class Judgement : Consumable
    {
        public Judgement(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        public override void Apply(GameContext context, int[] targetCards)
        {
            context.GlobalPoolManager.GenerateJoker(RngActionType.Judgement);
        }
        
        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // The judgement creates a random joker, must have space
            return ctx.JokerContainer.AvailableSlots > 0;
        }
    }

    public abstract class SuitChanger : Consumable
    {
        public SuitChanger(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }
        
        protected abstract Suit Suit { get; }
        
        private Card64 MakeSuit(Card64 card) => card.WithSuit(Suit);

        public override void Apply(GameContext context, int[] targetCards)
        {
            for (int i = 0; i < targetCards.Length; i++)
            {
                context.Hand.TransformCard(MakeSuit, targetCards[i]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            // Suit changers can be used on at most three cards
            return ctx.Hand.Count >= targetCards.Length
                   && targetCards.Max() < ctx.Hand.Count
                   && targetCards.Length > 0
                   && targetCards.Length < 3;
        }
    }
    
    [ConsumableStaticDescription(19, ConsumableType.Tarot)]
    public class TheStar : SuitChanger
    {
        public TheStar(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        protected override Suit Suit => Suit.Diamond;
    }
    
    [ConsumableStaticDescription(20, ConsumableType.Tarot)]
    public class TheMoon : SuitChanger
    {
        public TheMoon(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        protected override Suit Suit => Suit.Club;
    }
    
    [ConsumableStaticDescription(21, ConsumableType.Tarot)]
    public class TheSun : SuitChanger
    {
        public TheSun(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        protected override Suit Suit => Suit.Heart;
    }
    
    [ConsumableStaticDescription(22, ConsumableType.Tarot)]
    public class TheWorld : SuitChanger
    {
        public TheWorld(int staticId, uint id, bool isNegative = false) : base(staticId, id, isNegative)
        {
        }

        protected override Suit Suit => Suit.Spade;
    }
}