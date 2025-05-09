using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;

namespace Balatro.Core.GameEngine.GameStateController.EventBus
{
    public delegate void OnHandPlayed(Span<CardView> playedCardsViews, HandRank handRank);
    public delegate void OnHandDiscarded(Span<CardView> discardedCardsViews, HandRank handRank);

    public delegate void OnBlindSelected();
    
    public class GameEventBus
    {
        private OnHandPlayed? OnHandPlayed;
        private OnHandDiscarded? OnHandDiscarded;
        private OnBlindSelected? OnBlindSelected;
        
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
    }
}