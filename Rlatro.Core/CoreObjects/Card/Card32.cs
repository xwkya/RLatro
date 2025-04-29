namespace Balatro.Core.CoreObjects.Card
{
    /// <summary>
    /// Packed playing card representation. Holds inside 32 bits.
    /// </summary>
    public struct Card32
    {
        public uint Raw { get; private set; }

        private Card32(uint raw) => Raw = raw;

        // -- bit constants --
        private const int RANK_SHIFT = 0; // 4 bits
        private const int SUIT_SHIFT = 4; // 2 bits
        private const int ENH_SHIFT = 6; // 4 bits
        private const int SEAL_SHIFT = 10; // 3 bits
        private const int EDITION_SHIFT = 13; // 3 bits
        private const int CHIPS_UPGRADE_SHIFT = 16; // Reserve 12 bits

        private const uint RANK_MASK = 15 << RANK_SHIFT;
        private const uint SUIT_MASK = 3 << SUIT_SHIFT;
        private const uint ENH_MASK = 15 << ENH_SHIFT;
        private const uint SEAL_MASK = 7 << SEAL_SHIFT;
        private const uint EDITION_MASK = 7 << EDITION_SHIFT;
        private const uint CHIPS_UPGRADE_MASK = 4095 << CHIPS_UPGRADE_SHIFT;


        // -- static constructors --
        public static Card32 Create(Rank rank, Suit suit, Enhancement enh = Enhancement.None,
            Seal seal = Seal.None,
            Edition edition = Edition.None)
        {
            uint r = ((uint)rank & 0xF) << RANK_SHIFT |
                     ((uint)suit & 0x3) << SUIT_SHIFT |
                     ((uint)enh & 0xF) << ENH_SHIFT |
                     ((uint)seal & 0x7) << SEAL_SHIFT |
                     ((uint)edition & 0x7) << EDITION_SHIFT;
            return new Card32(r);
        }

        public static Card32 Create(uint raw) => new Card32(raw);

        // -- fast getters --
        public Rank GetRank() => (Rank)((Raw & RANK_MASK) >> RANK_SHIFT);
        public Suit GetSuit() => (Suit)((Raw & SUIT_MASK) >> SUIT_SHIFT);
        public Enhancement GetEnh() => (Enhancement)((Raw & ENH_MASK) >> ENH_SHIFT);
        public Seal GetSeal() => (Seal)((Raw & SEAL_MASK) >> SEAL_SHIFT);
        public Edition GetEdition() => (Edition)((Raw & EDITION_MASK) >> EDITION_SHIFT);
        public uint GetChipsUpgrade() => (Raw & CHIPS_UPGRADE_MASK) >> CHIPS_UPGRADE_SHIFT;
        public uint GetTotalChipsValue() => (GetRank().GetRankChips() + GetChipsUpgrade());

        // -- in-place setters --
        public void SetRank(Rank r) => Raw = (Raw & ~RANK_MASK) | (((uint)r & 0xF) << RANK_SHIFT);

        public void SetSuit(Suit s) => Raw = (Raw & ~SUIT_MASK) | (((uint)s & 0x3) << SUIT_SHIFT);

        public void SetEnhancement(Enhancement e) => Raw = (Raw & ~ENH_MASK) | (((uint)e & 0xF) << ENH_SHIFT);

        public void SetSeal(Seal s) => Raw = (Raw & ~SEAL_MASK) | (((uint)s & 0x7) << SEAL_SHIFT);

        public void SetEdition(Edition e) => Raw = (Raw & ~EDITION_MASK) | (((uint)e & 0x7) << EDITION_SHIFT);

        public void SetChipsUpgrade(uint chipUpgrade) =>
            Raw = (Raw & ~CHIPS_UPGRADE_MASK) | (chipUpgrade << CHIPS_UPGRADE_SHIFT);

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