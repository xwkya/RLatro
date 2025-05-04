using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreRules.CanonicalViews
{
    public readonly struct CardView
    {
        private const byte Face = 1 << 0; // Is the card **CONSIDERED** a face card
        private const byte RedSeal = 1 << 1;
        
        /// <summary>
        /// The rank of the underlying <see cref="Card32"/>.
        /// Current Balatro rules do not modify from the original card.
        /// </summary>
        public Rank Rank { get; init; }

        /// <summary>
        /// A bitmask of the suits of the underlying <see cref="Card64"/>.
        /// Some jokers or effects can affect the suit of the card (possibly consider it as all suits).
        /// </summary>
        public SuitMask Suits { get; init; }

        /// <summary>
        /// 8 special rule flags for specific effects.
        /// </summary>
        /// <code>
        /// var isFace = Face &amp; RuleFlags;
        /// </code>
        public byte RuleFlags { get; init; }
        
        public bool IsFace => (RuleFlags & Face) != 0;
        public bool IsRedSeal => (RuleFlags & RedSeal) != 0;
        
        public static CardView Create(Card64 card, GameContext ctx)
        {
            return new CardView()
            {
                Rank = card.GetRank(),
                Suits = GetSuits(card, ctx),
                RuleFlags = CreateRuleFlags(
                    isFace: card.GetRank().IsNaturalFaceCard() || ctx.JokerContainer.AllFaceCards(),
                    redSeal: card.GetSeal() == Seal.Red)
            };
        }
        
        public static CardView Create(Rank rank, SuitMask suits)
        {
            return new CardView()
            {
                Rank = rank,
                Suits = suits,
                RuleFlags = CreateRuleFlags(rank.IsNaturalFaceCard(), redSeal: false),
            };
        }

        private static byte CreateRuleFlags(bool isFace, bool redSeal)
        {
            byte flags = 0;
            if (isFace) flags |= Face;
            if (redSeal) flags |= RedSeal;
            return flags;
        }

        private static SuitMask GetSuits(Card64 card, GameContext ctx)
        {
            if (card.GetEnh() == Enhancement.Wild) return SuitMask.All;
            return (ctx.JokerContainer.GetJokersSuitChange(card.GetSuit()));
        }
    }
}