namespace Balatro.Core.CoreObjects.CardContainer
{
    public class Hand : CardContainer
    {
        // TODO: Use the game logic to determine the maximum hand size.
        public override int Capacity()
        {
            return 8;
        }

        private HashSet<int> selectedCards = new HashSet<int>();
    }
}