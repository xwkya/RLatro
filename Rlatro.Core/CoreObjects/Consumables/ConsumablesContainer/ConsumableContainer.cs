using Balatro.Core.CoreObjects.Consumables.ConsumableObject;

namespace Balatro.Core.CoreObjects.Consumables.ConsumablesContainer
{
    public class ConsumableContainer
    {
        public int Capacity { get; set; }
        public List<Consumable> Consumables { get; set; } = new List<Consumable>();

        public void RemoveConsumable(int index) => Consumables.RemoveAt(index);
        public void AddConsumable(Consumable consumable)
        {
            if (Consumables.Count >= Capacity)
                throw new InvalidOperationException("Consumable container is full.");
            
            Consumables.Add(consumable);
        }
    }
}