using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Cards.CardObject
{
    /// <summary>
    /// Packed playing card representation. Holds inside 32 bits.
    /// </summary>
    public struct Card32
    {
        public uint Raw { get; private set; }

        private Card32(uint raw) => Raw = raw;

        // -- bit constants --
        private const int RankShift = 0; // 4 bits
        private const int SuitShift = 4; // 2 bits
        private const int EnhShift = 6; // 4 bits
        private const int SealShift = 10; // 3 bits
        private const int EditionShift = 13; // 3 bits
        private const int ChipsUpgradeShift = 16; // Reserve 12 bits

        private const uint RankMask = 15 << RankShift;
        private const uint SuitMask = 3 << SuitShift;
        private const uint EnhMask = 15 << EnhShift;
        private const uint SealMask = 7 << SealShift;
        private const uint EditionMask = 7 << EditionShift;
        private const uint ChipsUpgradeMask = 4095 << ChipsUpgradeShift;


        // -- static constructors --
        public static Card32 Create(Rank rank, Suit suit, Enhancement enh = Enhancement.None,
            Seal seal = Seal.None,
            Edition edition = Edition.None)
        {
            uint r = ((uint)rank & 0xF) << RankShift |
                     ((uint)suit & 0x3) << SuitShift |
                     ((uint)enh & 0xF) << EnhShift |
                     ((uint)seal & 0x7) << SealShift |
                     ((uint)edition & 0x7) << EditionShift;
            return new Card32(r);
        }

        public static Card32 Create(uint raw) => new Card32(raw);

        // -- fast getters --
        public readonly Rank GetRank() => (Rank)((Raw & RankMask) >> RankShift);
        public readonly Suit GetSuit() => (Suit)((Raw & SuitMask) >> SuitShift);
        public readonly Enhancement GetEnh() => (Enhancement)((Raw & EnhMask) >> EnhShift);
        public readonly Seal GetSeal() => (Seal)((Raw & SealMask) >> SealShift);
        public readonly Edition GetEdition() => (Edition)((Raw & EditionMask) >> EditionShift);
        public readonly uint GetChipsUpgrade() => (Raw & ChipsUpgradeMask) >> ChipsUpgradeShift;
        public readonly uint GetTotalChipsValue() => (GetRank().GetRankChips() + GetChipsUpgrade());

        // -- in-place setters --
        public void SetRank(Rank r) => Raw = (Raw & ~RankMask) | (((uint)r & 0xF) << RankShift);

        public void SetSuit(Suit s) => Raw = (Raw & ~SuitMask) | (((uint)s & 0x3) << SuitShift);

        public void SetEnhancement(Enhancement e) => Raw = (Raw & ~EnhMask) | (((uint)e & 0xF) << EnhShift);

        public void SetSeal(Seal s) => Raw = (Raw & ~SealMask) | (((uint)s & 0x7) << SealShift);

        public void SetEdition(Edition e) => Raw = (Raw & ~EditionMask) | (((uint)e & 0x7) << EditionShift);

        public void SetChipsUpgrade(uint chipUpgrade) =>
            Raw = (Raw & ~ChipsUpgradeMask) | (chipUpgrade << ChipsUpgradeShift);

        public void AddChips(uint chips)
        {
            var chipUpgrade = GetChipsUpgrade();
            SetChipsUpgrade(chipUpgrade + chips);
        }

        public string Representation() =>
            $"{GetRank().ToString()} {GetSuit().ToString()} " +
            $"(Enhancement: {GetEnh().ToString()}, " +
            $"Seal: {GetSeal().ToString()}, " +
            $"Edition: {GetEdition().ToString()}, " +
            $"Total chips value: {GetTotalChipsValue()})";
    }
}