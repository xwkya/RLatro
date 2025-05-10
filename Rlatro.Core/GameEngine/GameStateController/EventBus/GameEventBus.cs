using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.CoreRules.CanonicalViews;


namespace Balatro.Core.GameEngine.GameStateController.EventBus
{
    public delegate void OnHandPlayed(ReadOnlySpan<CardView> playedCardsViews, HandRank handRank);

    public delegate void OnHandDiscarded(ReadOnlySpan<CardView> discardedCardsViews, HandRank handRank);

    public delegate void OnBlindSelected();

    public delegate void OnVoucherBought(VoucherType voucherType);

    /// <summary>
    /// Main event bus for the game instance. This verbose pattern is used to pass stack-allocated data.
    /// If we were to adopt a more common approach with dictionary of type and delegate, this would box the data when publishing.
    /// </summary>
    public class GameEventBus
    {
        private OnHandPlayed OnHandPlayed;
        private OnHandDiscarded OnHandDiscarded;
        private OnBlindSelected OnBlindSelected;
        private OnVoucherBought OnVoucherBought;

        #region HandPlayed

        public void SubscribeToHandPlayed(OnHandPlayed onHandPlayed)
        {
            OnHandPlayed += onHandPlayed;
        }

        public void UnsubscribeToHandPlayed(OnHandPlayed onHandPlayed)
        {
            OnHandPlayed -= onHandPlayed;
        }

        public void PublishHandPlayed(Span<CardView> playedCardsViews, HandRank handRank)
        {
            OnHandPlayed?.Invoke(playedCardsViews, handRank);
        }

        #endregion

        #region HandDiscarded

        public void SubscribeToHandDiscarded(OnHandDiscarded onHandDiscarded)
        {
            OnHandDiscarded += onHandDiscarded;
        }

        public void UnsubscribeToHandDiscarded(OnHandDiscarded onHandDiscarded)
        {
            OnHandDiscarded -= onHandDiscarded;
        }

        public void PublishHandDiscarded(Span<CardView> discardedCardsViews, HandRank handRank)
        {
            OnHandDiscarded?.Invoke(discardedCardsViews, handRank);
        }

        #endregion

        #region BlindSelected

        public void SubscribeToBlindSelected(OnBlindSelected onBlindSelected)
        {
            OnBlindSelected += onBlindSelected;
        }

        public void UnsubscribeToBlindSelected(OnBlindSelected onBlindSelected)
        {
            OnBlindSelected -= onBlindSelected;
        }

        public void PublishBlindSelected()
        {
            OnBlindSelected?.Invoke();
        }

        #endregion

        #region VoucherBought

        public void SubscribeToVoucherBought(OnVoucherBought onVoucherBought)
        {
            OnVoucherBought += onVoucherBought;
        }

        public void UnsubscribeToVoucherBought(OnVoucherBought onVoucherBought)
        {
            OnVoucherBought -= onVoucherBought;
        }

        public void PublishVoucherBought(VoucherType voucherType)
        {
            OnVoucherBought?.Invoke(voucherType);
        }

        #endregion
    }
}