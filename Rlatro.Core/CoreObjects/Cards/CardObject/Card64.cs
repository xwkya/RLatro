using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Cards.CardObject
{
    /// <summary>
    /// Packed playing card representation. Holds inside 64 bits.
    /// </summary>
    public readonly struct Card64
    {
        private readonly uint Raw;
        public readonly uint Id;

        private Card64(uint raw, uint id)
        {
            Raw = raw;
            Id = id;
        }

        // -- bit constants --
        private const int RankShift = 0; // 4 bits
        private const int SuitShift = 4; // 2 bits (cumulative -> 6 bits)
        private const int EnhShift = 6; // 4 bits (cumulative -> 10 bits)
        private const int SealShift = 10; // 3 bits (cumulative -> 13 bits)
        private const int EditionShift = 13; // 3 bits (cumulative -> 16 bits)
        private const int ChipsUpgradeShift = 16; // Realistically chips upgrades will never be more than 1024, reserve 10 bits (cumulative -> 26 bits)
        // 6 bits left for any potential future use

        private const uint RankMask = 15 << RankShift;
        private const uint SuitMask = 3 << SuitShift;
        private const uint EnhMask = 15 << EnhShift;
        private const uint SealMask = 7 << SealShift;
        private const uint EditionMask = 7 << EditionShift;
        private const uint ChipsUpgradeMask = 1023 << ChipsUpgradeShift;


        // -- static constructors --
        public static Card64 Create(
            uint id,
            Rank rank,
            Suit suit,
            Enhancement enh = Enhancement.None,
            Seal seal = Seal.None,
            Edition edition = Edition.None)
        {
            uint r = ((uint)rank & 0xF) << RankShift |
                     ((uint)suit & 0x3) << SuitShift |
                     ((uint)enh & 0xF) << EnhShift |
                     ((uint)seal & 0x7) << SealShift |
                     ((uint)edition & 0x7) << EditionShift;
            
            return new Card64(r, id);
        }

        public static Card64 Create(uint raw, uint id) => new Card64(raw, id);

        // -- fast getters --
        public Rank GetRank() => (Rank)((Raw & RankMask) >> RankShift);
        public Suit GetSuit() => (Suit)((Raw & SuitMask) >> SuitShift);
        public Enhancement GetEnh() => (Enhancement)((Raw & EnhMask) >> EnhShift);
        public Seal GetSeal() => (Seal)((Raw & SealMask) >> SealShift);
        public Edition GetEdition() => (Edition)((Raw & EditionMask) >> EditionShift);
        public uint GetChipsUpgrade() => (Raw & ChipsUpgradeMask) >> ChipsUpgradeShift;
        public uint GetTotalChipsValue() => (GetRank().GetRankChips() + GetChipsUpgrade());
        public uint GetRaw() => Raw;

        // -- readonly setters --
        public Card64 WithRank(Rank r)
        {
            var raw = (Raw & ~RankMask) | (((uint)r & 0xF) << RankShift);
            return new Card64(raw, Id);
        }

        public Card64 WithSuit(Suit s)
        {
            var raw = (Raw & ~SuitMask) | (((uint)s & 0x3) << SuitShift);
            return new Card64(raw, Id);
        }

        public Card64 WithEnhancement(Enhancement e)
        {
            var raw = (Raw & ~EnhMask) | (((uint)e & 0xF) << EnhShift);
            return new Card64(raw, Id);
        }

        public Card64 WithSeal(Seal s)
        {
            var raw = (Raw & ~SealMask) | (((uint)s & 0x7) << SealShift);
            return new Card64(raw, Id);
        }

        public Card64 WithEdition(Edition e)
        {
            var raw = (Raw & ~EditionMask) | (((uint)e & 0x7) << EditionShift);
            return new Card64(raw, Id);
        }

        public Card64 WithChipsUpgrade(uint chipUpgrade)
        {
            var raw = (Raw & ~ChipsUpgradeMask) | (chipUpgrade << ChipsUpgradeShift);
            return new Card64(raw, Id);
        }

        public Card64 WithAddChips(uint chips)
        {
            var chipUpgrade = GetChipsUpgrade();
            return WithChipsUpgrade(chipUpgrade + chips);
        }

        public string Representation() =>
            $"{GetRank().ToString()} {GetSuit().ToString()} " +
            $"(Enhancement: {GetEnh().ToString()}, " +
            $"Seal: {GetSeal().ToString()}, " +
            $"Edition: {GetEdition().ToString()}, " +
            $"Total chips value: {GetTotalChipsValue()})";
    }
}