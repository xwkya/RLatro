using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.ObjectsImplementations.Consumables
{
    [ConsumableStaticDescription(100, ConsumableType.Spectral)]
    public class Familiar : Consumable
    {
        public Familiar(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Destroy 1 random card in hand using deterministic RNG
            if (context.Hand.Count > 0)
            {
                Span<int> handIndexes = stackalloc int[context.Hand.Count];
                uint[] handKeys = new uint[context.Hand.Count];
                
                for (int i = 0; i < context.Hand.Count; i++)
                {
                    handIndexes[i] = i;
                    handKeys[i] = context.Hand.Span[i].Id;
                }
                
                var randomIndex = context.RngController.GetRandomIndexContiguous(handIndexes, handKeys, RngActionType.SpectralFamiliar);
                context.Hand.Remove(randomIndex);
            }

            // Add 3 random enhanced face cards
            for (int i = 0; i < 3; i++)
            {
                var randomRank = RankExtensions.RandomFaceRank(context.RngController, RngActionType.SpectralFamiliar);
                var randomSuit = (Suit)context.RngController.RandomInt(0, 3, RngActionType.SpectralFamiliar);
                var randomEnhancement = EnhancementExtensions.GetRandomEnhancement(context.RngController, RngActionType.SpectralFamiliar);
                
                var card = context.CoreObjectsFactory.CreateCard(randomRank, randomSuit, randomEnhancement);
                context.Hand.Add(card);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.Hand.Count > 0;
        }
    }

    [ConsumableStaticDescription(101, ConsumableType.Spectral)]
    public class Grim : Consumable
    {
        public Grim(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Destroy 1 random card in hand using deterministic RNG
            if (context.Hand.Count > 0)
            {
                Span<int> handIndexes = stackalloc int[context.Hand.Count];
                uint[] handKeys = new uint[context.Hand.Count];
                
                for (int i = 0; i < context.Hand.Count; i++)
                {
                    handIndexes[i] = i;
                    handKeys[i] = context.Hand.Span[i].Id;
                }
                
                var randomIndex = context.RngController.GetRandomIndexContiguous(handIndexes, handKeys, RngActionType.SpectralGrim);
                context.Hand.Remove(randomIndex);
            }

            // Add 2 random enhanced Aces
            for (int i = 0; i < 2; i++)
            {
                var randomSuit = (Suit)context.RngController.RandomInt(0, 3, RngActionType.SpectralGrim);
                var randomEnhancement = EnhancementExtensions.GetRandomEnhancement(context.RngController, RngActionType.SpectralGrim);
                
                var card = context.CoreObjectsFactory.CreateCard(Rank.Ace, randomSuit, randomEnhancement);
                context.Hand.Add(card);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.Hand.Count > 0;
        }
    }

    [ConsumableStaticDescription(102, ConsumableType.Spectral)]
    public class Incantation : Consumable
    {
        public Incantation(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Destroy 1 random card in hand using deterministic RNG
            if (context.Hand.Count > 0)
            {
                Span<int> handIndexes = stackalloc int[context.Hand.Count];
                uint[] handKeys = new uint[context.Hand.Count];
                
                for (int i = 0; i < context.Hand.Count; i++)
                {
                    handIndexes[i] = i;
                    handKeys[i] = context.Hand.Span[i].Id;
                }
                
                var randomIndex = context.RngController.GetRandomIndexContiguous(handIndexes, handKeys, RngActionType.SpectralIncantation);
                context.Hand.Remove(randomIndex);
            }

            // Add 4 random enhanced numbered cards (2-10)
            for (int i = 0; i < 4; i++)
            {
                var randomRank = RankExtensions.RandomNumberedRank(context.RngController, RngActionType.SpectralIncantation);
                var randomSuit = (Suit)context.RngController.RandomInt(0, 3, RngActionType.SpectralIncantation);
                var randomEnhancement = EnhancementExtensions.GetRandomEnhancement(context.RngController, RngActionType.SpectralIncantation);
                
                var card = context.CoreObjectsFactory.CreateCard(randomRank, randomSuit, randomEnhancement);
                context.Hand.Add(card);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.Hand.Count > 0;
        }
    }

    [ConsumableStaticDescription(103, ConsumableType.Spectral)]
    public class Talisman : Consumable
    {
        public Talisman(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (targetCards.Length > 0 && targetCards[0] < context.Hand.Count)
            {
                context.Hand.TransformCard(card => card.WithSeal(Seal.Gold), targetCards[0]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return targetCards.Length == 1 && targetCards[0] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(104, ConsumableType.Spectral)]
    public class Aura : Consumable
    {
        public Aura(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (targetCards.Length > 0 && targetCards[0] < context.Hand.Count)
            {
                var editions = new[] { Edition.Foil, Edition.Holo, Edition.Poly };
                var randomEdition = editions[context.RngController.RandomInt(0, editions.Length - 1, RngActionType.SpectralAura)];
                
                context.Hand.TransformCard(card => card.WithEdition(randomEdition), targetCards[0]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return targetCards.Length == 1 && targetCards[0] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(105, ConsumableType.Spectral)]
    public class Wraith : Consumable
    {
        public Wraith(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Create a random Rare Joker
            var rareJoker = context.GlobalPoolManager.GenerateJoker(RngActionType.SpectralWraith, JokerRarity.Rare);
            context.JokerContainer.AddJoker(context, rareJoker);
            
            // Set money to $0
            var currentGold = context.PersistentState.EconomyHandler.GetCurrentGold();
            context.PersistentState.EconomyHandler.SpendGold(currentGold);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.JokerContainer.Jokers.Count < ctx.JokerContainer.Slots;
        }
    }

    [ConsumableStaticDescription(106, ConsumableType.Spectral)]
    public class Sigil : Consumable
    {
        public Sigil(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (context.Hand.Count == 0) return;
            
            // Choose a random suit
            var randomSuit = (Suit)context.RngController.RandomInt(0, 3, RngActionType.SpectralSigil);
            
            // Convert all cards in hand to this suit
            for (int i = 0; i < context.Hand.Count; i++)
            {
                context.Hand.TransformCard(card => card.WithSuit(randomSuit), i);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.Hand.Count > 0;
        }
    }

    [ConsumableStaticDescription(107, ConsumableType.Spectral)]
    public class Ouija : Consumable
    {
        public Ouija(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (context.Hand.Count == 0) return;
            
            // Choose a random rank
            var randomRank = (Rank)context.RngController.RandomInt(0, 12, RngActionType.SpectralOuija);
            
            // Convert all cards in hand to this rank
            for (int i = 0; i < context.Hand.Count; i++)
            {
                context.Hand.TransformCard(card => card.WithRank(randomRank), i);
            }
            
            // Decrease hand size by 1
            context.PersistentState.HandSize--;
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.Hand.Count > 0 && ctx.PersistentState.HandSize > 1;
        }
    }

    [ConsumableStaticDescription(108, ConsumableType.Spectral)]
    public class Ectoplasm : Consumable
    {
        public Ectoplasm(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Find jokers without Negative edition using deterministic selection
            var eligibleJokers = new List<int>();
            for (int i = 0; i < context.JokerContainer.Jokers.Count; i++)
            {
                if (context.JokerContainer.Jokers[i].Edition != Edition.Negative)
                {
                    eligibleJokers.Add(i);
                }
            }
            
            if (eligibleJokers.Count > 0)
            {
                Span<int> eligibleIndexes = stackalloc int[eligibleJokers.Count];
                uint[] eligibleKeys = new uint[eligibleJokers.Count];
                
                for (int i = 0; i < eligibleJokers.Count; i++)
                {
                    eligibleIndexes[i] = eligibleJokers[i];
                    eligibleKeys[i] = context.JokerContainer.Jokers[eligibleJokers[i]].Id;
                }
                
                var randomIndex = context.RngController.GetRandomIndexNonContiguous(eligibleIndexes, eligibleKeys, RngActionType.SpectralEctoplasm);
                context.JokerContainer.Jokers[randomIndex].Edition = Edition.Negative;
            }
            
            // Decrease hand size (cumulative: current usage count + 1)
            context.PersistentState.EctoplasmUsageCount++;
            context.PersistentState.HandSize -= context.PersistentState.EctoplasmUsageCount;
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.JokerContainer.Jokers.Any(j => j.Edition != Edition.Negative) && ctx.PersistentState.HandSize > 1;
        }
    }

    [ConsumableStaticDescription(109, ConsumableType.Spectral)]
    public class Immolate : Consumable
    {
        public Immolate(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Destroy up to 5 random cards in hand using shuffle approach
            var cardsToDestroy = Math.Min(5, context.Hand.Count);
            if (cardsToDestroy > 0)
            {
                Span<int> handIndexes = stackalloc int[context.Hand.Count];
                uint[] handKeys = new uint[context.Hand.Count];
                
                for (int i = 0; i < context.Hand.Count; i++)
                {
                    handIndexes[i] = i;
                    handKeys[i] = context.Hand.Span[i].Id;
                }
                
                // Shuffle the indexes using the card IDs as keys
                context.RngController.GetShuffleContiguous(handIndexes, handKeys, RngActionType.SpectralImmolate);
                
                // Take the first cardsToDestroy indexes and sort them descending for safe removal
                var indicesToRemove = handIndexes.Slice(0, cardsToDestroy).ToArray();
                Array.Sort(indicesToRemove, (a, b) => b.CompareTo(a));
                
                context.Hand.Remove(indicesToRemove);
            }
            
            // Gain $20
            context.PersistentState.EconomyHandler.AddGold(20);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.Hand.Count > 0;
        }
    }

    [ConsumableStaticDescription(110, ConsumableType.Spectral)]
    public class Ankh : Consumable
    {
        public Ankh(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (context.JokerContainer.Jokers.Count == 0) return;
            
            // Choose a random joker to copy using deterministic selection
            Span<int> jokerIndexes = stackalloc int[context.JokerContainer.Jokers.Count];
            uint[] jokerKeys = new uint[context.JokerContainer.Jokers.Count];
            
            for (int i = 0; i < context.JokerContainer.Jokers.Count; i++)
            {
                jokerIndexes[i] = i;
                jokerKeys[i] = context.JokerContainer.Jokers[i].Id;
            }
            
            var randomIndex = context.RngController.GetRandomIndexContiguous(jokerIndexes, jokerKeys, RngActionType.SpectralAnkh);
            var jokerToCopy = context.JokerContainer.Jokers[randomIndex];
            
            // Create a copy (edition copied except Negative)
            var copyEdition = jokerToCopy.Edition == Edition.Negative ? Edition.None : jokerToCopy.Edition;
            var copiedJoker = context.CoreObjectsFactory.CreateJoker(jokerToCopy.StaticId, copyEdition);
            
            // Copy all properties except ID
            copiedJoker.Scaling = jokerToCopy.Scaling;
            copiedJoker.Suit = jokerToCopy.Suit;
            copiedJoker.Rank = jokerToCopy.Rank;
            copiedJoker.BonusSellValue = jokerToCopy.BonusSellValue;
            
            // Remove all jokers except the chosen one
            for (int i = context.JokerContainer.Jokers.Count - 1; i >= 0; i--)
            {
                if (i != randomIndex)
                {
                    var jokerStaticId = context.JokerContainer.Jokers[i].StaticId;
                    context.JokerContainer.RemoveJoker(context, i);
                    context.GameEventBus.PublishJokerRemovedFromContext(jokerStaticId);
                    // Adjust the index if we removed a joker before the chosen one
                    if (i < randomIndex) randomIndex--;
                }
            }
            
            // Add the copy
            context.JokerContainer.AddJoker(context, copiedJoker);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.JokerContainer.Jokers.Count > 0;
        }
    }

    [ConsumableStaticDescription(111, ConsumableType.Spectral)]
    public class DejaVu : Consumable
    {
        public DejaVu(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (targetCards.Length > 0 && targetCards[0] < context.Hand.Count)
            {
                context.Hand.TransformCard(card => card.WithSeal(Seal.Red), targetCards[0]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return targetCards.Length == 1 && targetCards[0] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(112, ConsumableType.Spectral)]
    public class Hex : Consumable
    {
        public Hex(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (context.JokerContainer.Jokers.Count == 0) return;
            
            // Choose a random joker to add Polychrome to using deterministic selection
            Span<int> jokerIndexes = stackalloc int[context.JokerContainer.Jokers.Count];
            uint[] jokerKeys = new uint[context.JokerContainer.Jokers.Count];
            
            for (int i = 0; i < context.JokerContainer.Jokers.Count; i++)
            {
                jokerIndexes[i] = i;
                jokerKeys[i] = context.JokerContainer.Jokers[i].Id;
            }
            
            var randomIndex = context.RngController.GetRandomIndexContiguous(jokerIndexes, jokerKeys, RngActionType.SpectralHex);
            context.JokerContainer.Jokers[randomIndex].Edition = Edition.Poly;
            
            // Remove all other jokers
            for (int i = context.JokerContainer.Jokers.Count - 1; i >= 0; i--)
            {
                if (i != randomIndex)
                {
                    var jokerStaticId = context.JokerContainer.Jokers[i].StaticId;
                    context.JokerContainer.RemoveJoker(context, i);
                    context.GameEventBus.PublishJokerRemovedFromContext(jokerStaticId);
                    // Adjust the index if we removed a joker before the chosen one
                    if (i < randomIndex) randomIndex--;
                }
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.JokerContainer.Jokers.Count > 0;
        }
    }

    [ConsumableStaticDescription(113, ConsumableType.Spectral)]
    public class Trance : Consumable
    {
        public Trance(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (targetCards.Length > 0 && targetCards[0] < context.Hand.Count)
            {
                context.Hand.TransformCard(card => card.WithSeal(Seal.Blue), targetCards[0]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return targetCards.Length == 1 && targetCards[0] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(114, ConsumableType.Spectral)]
    public class Medium : Consumable
    {
        public Medium(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (targetCards.Length > 0 && targetCards[0] < context.Hand.Count)
            {
                context.Hand.TransformCard(card => card.WithSeal(Seal.Purple), targetCards[0]);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return targetCards.Length == 1 && targetCards[0] < ctx.Hand.Count;
        }
    }

    [ConsumableStaticDescription(115, ConsumableType.Spectral)]
    public class Cryptid : Consumable
    {
        public Cryptid(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            if (targetCards.Length > 0 && targetCards[0] < context.Hand.Count)
            {
                var originalCard = context.Hand.Span[targetCards[0]];
                
                // Create 2 exact copies
                for (int i = 0; i < 2; i++)
                {
                    var copy = Card64.Create(
                        raw: originalCard.GetRaw(),
                        id: context.CoreObjectsFactory.GetNextRuntimeId()
                    ).WithChipsUpgrade(originalCard.GetChipsUpgrade());
                    
                    context.Hand.Add(copy);
                }
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return targetCards.Length == 1 && targetCards[0] < ctx.Hand.Count;
        }
    }
    
    [ConsumableStaticDescription(116, ConsumableType.Spectral)]
    public class TheSoul : Consumable
    {
        public TheSoul(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Create a Legendary Joker
            var legendaryJoker = context.GlobalPoolManager.GenerateJoker(RngActionType.SpectralTheSoul, JokerRarity.Legendary);
            context.JokerContainer.AddJoker(context, legendaryJoker);
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return ctx.JokerContainer.Jokers.Count < ctx.JokerContainer.Slots;
        }
    }

    [ConsumableStaticDescription(117, ConsumableType.Spectral)]
    public class BlackHole : Consumable
    {
        public BlackHole(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, int[] targetCards)
        {
            // Upgrade every poker hand by one level
            foreach (HandRank handRank in Enum.GetValues<HandRank>())
            {
                context.PersistentState.HandTracker.UpgradeHand(handRank);
            }
        }

        public override bool IsUsable(GameContext ctx, int[] targetCards)
        {
            return true;
        }
    }
}