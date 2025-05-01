namespace Balatro.Core.CoreRules.CanonicalViews
{
    [Flags]
    public enum SuitMask : byte
    {
        None = 0,
        Spade = 1,
        Heart = 2,
        Club = 4,
        Diamond = 8,
        All = Spade | Heart | Club | Diamond,
    }
}