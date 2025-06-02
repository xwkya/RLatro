using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PersistentStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Registries
{
    public static class CardRegistry
    {
        public static Card64 CreateCard(GameContext ctx, bool fromPack, string suffix = "")
        {
            var cardPollActionType = fromPack ? RngActionType.PackCardBase : RngActionType.ShopCardBase;
            var enhancementActionType = fromPack ? RngActionType.PackCardEnhancement : RngActionType.ShopCardEnhancement;
            var editionActionType = fromPack ? RngActionType.PackCardEdition : RngActionType.ShopCardEdition;
            var sealActionType = fromPack ? RngActionType.PackCardSeal : RngActionType.ShopCardSeal;
            
            var rank = GetRandomRank(ctx.RngController, cardPollActionType, suffix);
            var suit = GetRandomSuit(ctx.RngController, cardPollActionType, suffix);
            
            // If the card is from the shop, it cannot have an edition, enhancement or seal unless illusion is bought
            // Illusion is currently bugged and seals are not supported even if the voucher is owned
            var illusionOwned = ctx.PersistentState.OwnedVouchers[(int)VoucherType.Illusion];
            
            var shouldPollEdition = fromPack || illusionOwned;
            var shouldPollEnhancement = fromPack || illusionOwned;
            var shouldPollSeal = fromPack;
            
            var edition = shouldPollEdition ? PollEdition(ctx.PersistentState.AppearanceRates, ctx.RngController, editionActionType, suffix) : Edition.None;
            var enhancement = shouldPollEnhancement ? PollEnhancement(ctx.PersistentState.AppearanceRates, ctx.RngController, enhancementActionType, suffix) : Enhancement.None;
            var seal = shouldPollSeal ? PollSeal(ctx.PersistentState.AppearanceRates, ctx.RngController, sealActionType, suffix) : Seal.None;

            return ctx.CoreObjectsFactory.CreateCard(rank, suit, enhancement, seal, edition);
        }
        
        private static Rank GetRandomRank(RngController rngController, RngActionType actionType, string suffix = "")
        {
            var numberOfRanks = Enum.GetValues<Rank>().Length;
            var randomIndex = rngController.RandomInt(0, numberOfRanks - 1, actionType, suffix);
            
            return (Rank)randomIndex;
        }
        
        private static Suit GetRandomSuit(RngController rngController, RngActionType actionType, string suffix = "")
        {
            var numberOfSuits = Enum.GetValues<Suit>().Length;
            var randomIndex = rngController.RandomInt(0, numberOfSuits - 1, actionType, suffix);
            
            return (Suit)randomIndex;
        }

        private static Edition PollEdition(AppearanceRates rates, RngController rngController, RngActionType actionType, string suffix = "")
        {
            var randomValue = rngController.GetRandomProbability(actionType, suffix);
            if (randomValue < rates.FoilPlayingCardRate)
            {
                return Edition.Foil;
            }
            
            if (randomValue < rates.FoilPlayingCardRate + rates.PolychromePlayingCardRate)
            {
                return Edition.Poly;
            }
            
            if (randomValue < rates.FoilPlayingCardRate + rates.PolychromePlayingCardRate + rates.HoloPlayingCardRate)
            {
                return Edition.Holo;
            }

            return Edition.None;
        }

        private static Enhancement PollEnhancement(AppearanceRates rates, RngController rngController,
            RngActionType actionType, string suffix = "")
        {
            var randomValue = rngController.GetRandomProbability(actionType, suffix);
            if (randomValue < rates.EnhancementChanceRate)
            {
                return GetRandomEnhancement(rngController, actionType, suffix);
            }

            return Enhancement.None;
        }

        private static Seal PollSeal(AppearanceRates rates, RngController rngController,
            RngActionType actionType, string suffix = "")
        {
            var randomValue = rngController.GetRandomProbability(actionType, suffix);
            if (randomValue < rates.SealChanceRate)
            {
                return GetRandomSeal(rngController, actionType, suffix);
            }

            return Seal.None;
        }
        
        // TODO: Add unit tests for these to make sure 0 => None
        private static Enhancement GetRandomEnhancement(RngController rngController, RngActionType actionType, string suffix = "")
        {
            var numberOfEnhancements = Enum.GetValues<Enhancement>().Length;
            var randomIndex = rngController.RandomInt(1, numberOfEnhancements - 1, actionType, suffix);
            
            // Poll between 1 and max value. 0 is for None.
            return (Enhancement)randomIndex;
        }

        private static Seal GetRandomSeal(RngController rngController, RngActionType actionType, string suffix = "")
        {
            var numberOfSeals = Enum.GetValues<Seal>().Length;
            var randomIndex = rngController.RandomInt(1, numberOfSeals - 1, actionType, suffix);
            
            // Poll between 1 and max value. 0 is for no seal.
            return (Seal)randomIndex;
        }
    }
}