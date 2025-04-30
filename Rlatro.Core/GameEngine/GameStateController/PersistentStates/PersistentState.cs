namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class PersistentState
    {
        public uint Gold { get; set; }
        public byte Discards { get; set; }
        public byte Hands { get; set; }
        public byte HandSize { get; set; }
    }
}