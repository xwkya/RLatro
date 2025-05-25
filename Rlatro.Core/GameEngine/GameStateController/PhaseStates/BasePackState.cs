using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public abstract class BasePackState<T> : BaseGamePhaseState 
        where T : BasePackState<T>
    {
        private IGamePhaseState IncomingState { get; set; }
        protected int PackSize { get; set; }
        protected int NumberOfChoices { get; set; }
        
        public BasePackState(GameContext ctx) : base(ctx)
        {
            
        }
        
        public T SetIncomingState(IGamePhaseState incomingState)
        {
            IncomingState = incomingState;
            return (T)this;
        }

        public T SetPackType(BoosterPackType type)
        {
            var t = type.GetPackSizeAndChoice();
            PackSize = t.size;
            NumberOfChoices = t.choice;
            return (T)this;
        }
        
        public override GamePhase Phase => GamePhase.ArcanaPack;
        public override bool ShouldInitializeNextState => false;
        
        public override IGamePhaseState GetNextPhaseState()
        {
            return IncomingState;
        }
    }
}