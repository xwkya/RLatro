using Balatro.Core.CoreObjects.BoosterPacks;

namespace Balatro.Core.CoreObjects.Shop.ShopContainers
{
    public class BoosterContainer
    {
        public const int BoosterPackSlots = 2;
        public List<BoosterPack> BoosterPacks { get; } = new List<BoosterPack>();
        
        public void AddPack(BoosterPack pack)
        {
            BoosterPacks.Add(pack);
        }
    }
}