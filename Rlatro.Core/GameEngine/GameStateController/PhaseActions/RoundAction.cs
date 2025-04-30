namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public enum RoundActionIntent
    {
        Play,
        Discard,
        UseConsumable,
        SellConsumable,
        SellJoker,
    }

    public static class RoundActionExtensions
    {
        public static bool IsParameterlessAction(this RoundActionIntent actionIntent)
        {
            return actionIntent == RoundActionIntent.Play || actionIntent == RoundActionIntent.Discard || actionIntent == RoundActionIntent.UseConsumable;
        }
    }
    
    /// <summary>
    /// A round action is an action that can be performed during a round.
    /// There are two types of actions:
    /// <list type="bullet">
    /// <item>Actions on objects: Highlight cards, Sell Consumable, Sell Joker, Use Consumable (they will target highlighted cards)</item>
    /// <item>Parameterless actions: Play, Discard</item>
    /// </list>
    /// </summary>
    public sealed class RoundAction : BasePlayerAction
    {
        public RoundActionIntent ActionIntent { get; init; }
        public byte[] CardIndexes { get; init; }
        public byte ConsumableIndex { get; init; }
        public byte JokerIndex { get; init; }
    }
}