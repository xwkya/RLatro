// See https://aka.ms/new-console-template for more information

using Balatro.Core.CoreObjects.Card;
using Balatro.Core.CoreObjects.CardContainer;
using Balatro.Core.CoreObjects.CardContainer.DeckImplementation;

var deck = new Deck();
var hand = new Hand();
var discardPile = new DiscardPile();


deck.AddMany(new List<Card32>
{
    Card32.Create(Rank.Ace, Suit.Diamond),
    Card32.Create(Rank.Two, Suit.Diamond),
    Card32.Create(Rank.Three, Suit.Diamond),
    Card32.Create(Rank.Four, Suit.Diamond),
    Card32.Create(Rank.Five, Suit.Diamond),
});

deck.MoveAllCardsTo(hand);

Console.WriteLine("Cards in hand:");
Console.Write(hand.Display());
Console.WriteLine("Cards in deck:");
Console.Write(deck.Display());

hand.TransformCard(
    x =>
    {
        x.SetEdition(Edition.Foil);
        return x;
    }, 2);

Console.Write(hand.Display());
