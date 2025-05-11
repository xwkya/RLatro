using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.CoreRules.CanonicalViews;


namespace Balatro.Core.GameEngine.GameStateController.EventBus
{
    public delegate void OnHandPlayed(ReadOnlySpan<CardView> playedCardsViews, HandRank handRank);
    public delegate void OnHandDiscarded(ReadOnlySpan<CardView> discardedCardsViews, HandRank handRank);
    public delegate void OnBlindSelected();
    public delegate void OnVoucherBought(VoucherType voucherType);
    public delegate void OnJokerAddedToContext(int staticId);
    public delegate void OnJokerRemovedFromContext(int staticId);
    public delegate void OnConsumableAddedToContext(int staticId);
    public delegate void OnConsumableRemovedFromContext(int staticId);

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
        private OnJokerAddedToContext OnJokerAddedToContext;
        private OnJokerRemovedFromContext OnJokerRemovedFromContext;
        private OnConsumableAddedToContext OnConsumableAddedToContext;
        private OnConsumableRemovedFromContext OnConsumableRemovedFromContext;
        
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
        
        #region JokerAddedToContext
        
        public void SubscribeToJokerAddedToContext(OnJokerAddedToContext onJokerAddedToContext)
        {
            OnJokerAddedToContext += onJokerAddedToContext;
        }
        
        public void UnsubscribeToJokerAddedToContext(OnJokerAddedToContext onJokerAddedToContext)
        {
            OnJokerAddedToContext -= onJokerAddedToContext;
        }
        
        public void PublishJokerAddedToContext(int staticId)
        {
            OnJokerAddedToContext?.Invoke(staticId);
        }
        
        #endregion
        
        #region JokerRemovedFromContext
        
        public void SubscribeToJokerRemovedFromContext(OnJokerRemovedFromContext onJokerRemovedFromContext)
        {
            OnJokerRemovedFromContext += onJokerRemovedFromContext;
        }
        
        public void UnsubscribeToJokerRemovedFromContext(OnJokerRemovedFromContext onJokerRemovedFromContext)
        {
            OnJokerRemovedFromContext -= onJokerRemovedFromContext;
        }
        
        public void PublishJokerRemovedFromContext(int staticId)
        {
            OnJokerRemovedFromContext?.Invoke(staticId);
        }
        
        #endregion
        
        #region ConsumableAddedToContext
        
        public void SubscribeToConsumableAddedToContext(OnConsumableAddedToContext onConsumableAddedToContext)
        {
            OnConsumableAddedToContext += onConsumableAddedToContext;
        }
        
        public void UnsubscribeToConsumableAddedToContext(OnConsumableAddedToContext onConsumableAddedToContext)
        {
            OnConsumableAddedToContext -= onConsumableAddedToContext;
        }
        
        public void PublishConsumableAddedToContext(int staticId)
        {
            OnConsumableAddedToContext?.Invoke(staticId);
        }
        
        #endregion
        
        #region ConsumableRemovedFromContext
        
        public void SubscribeToConsumableRemovedFromContext(OnConsumableRemovedFromContext onConsumableRemovedFromContext)
        {
            OnConsumableRemovedFromContext += onConsumableRemovedFromContext;
        }
        
        public void UnsubscribeToConsumableRemovedFromContext(OnConsumableRemovedFromContext onConsumableRemovedFromContext)
        {
            OnConsumableRemovedFromContext -= onConsumableRemovedFromContext;
        }
        
        public void PublishConsumableRemovedFromContext(int staticId)
        {
            OnConsumableRemovedFromContext?.Invoke(staticId);
        }
        
        #endregion
    }
}