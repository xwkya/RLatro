namespace Balatro.Core.GameEngine.PseudoRng
{
    public enum RngActionType
    {
        LuckyCardMoney,
        LuckyCardMult,
        Shuffle,
        WheelOfFortune,
        GrosMichel,
        Cavendish,
        SpaceJoker
    }
    
    static class RngActionTypeExt
    {
        private static readonly Dictionary<RngActionType, string> ActionTypeToKey = new()
        {
            { RngActionType.LuckyCardMoney, "lucky_money" },
            { RngActionType.LuckyCardMult,  "lucky_mult"  },
            { RngActionType.Shuffle,        "shuffle"     },
            { RngActionType.WheelOfFortune, "wheel_of_fortune" },
            { RngActionType.GrosMichel, "gros_michel"},
            { RngActionType.Cavendish , "cavendish"},
            { RngActionType.SpaceJoker, "space"}
        };
        public static string Key(this RngActionType a) => a switch
        {
            RngActionType.LuckyCardMoney => "lucky_money",
            RngActionType.LuckyCardMult  => "lucky_mult",
            RngActionType.Shuffle        => "shuffle",
            RngActionType.WheelOfFortune => "wheel_of_fortune",
            _ => throw new ArgumentOutOfRangeException(nameof(a))
        };
    }
}