namespace Balatro.Core.GameEngine.GameStateController.EventBus
{
    public interface IEventBusSubscriber
    {
        public void Subscribe(GameEventBus eventBus);
    }
}