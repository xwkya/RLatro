using Balatro.Core.CoreObjects.Consumables.ConsumableObject;

namespace Balatro.Core.CoreObjects.Consumables.ConsumablesContainer
{
    public class ConsumableContainer
    {
        public int Capacity { get; set; }
        
        public List<Consumable> Consumables { get; set; }

        public void RemoveConsumable(byte index) => Consumables.RemoveAt(index);
    }
}