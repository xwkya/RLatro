using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.CoreRules.Modifiers
{
    /// <summary>
    /// Handler vouchers instant effects such as Hierophlyph or Overstock
    /// </summary>
    public class VoucherEffectHandler : IEventBusSubscriber
    {
        private readonly GameContext GameContext;
        
        public VoucherEffectHandler(GameContext gameContext)
        {
            GameContext = gameContext;
        }
        
        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToVoucherBought(OnVoucherBought);
        }
        
        private void OnVoucherBought(VoucherType voucherType)
        {
            switch (voucherType)
            {
                case VoucherType.Overstock:
                case VoucherType.OverstockPlus:
                    // Immediate effect: restock the shop
                    GameContext.GetPhase<ShopState>().OverstockFill(voucherType);
                    break;
                    
                case VoucherType.Hieroglyph:
                case VoucherType.Petroglyph:
                    // Immediate effect: go back 1 ante (reduce round by 3)
                    GameContext.PersistentState.Round = Math.Max(1, GameContext.PersistentState.Round - 3);
                    break;
                case VoucherType.Antimatter:
                    GameContext.JokerContainer.Slots++;
                    break;
                case VoucherType.CrystalBall:
                    GameContext.ConsumableContainer.Capacity++;
                    break;
            }
        }
    }
}