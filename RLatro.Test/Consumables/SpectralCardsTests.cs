using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.ObjectsImplementations.Consumables;
using Balatro.Core.ObjectsImplementations.Decks;

namespace RLatro.Test.Consumables;

public static class SpectralTestHelper
{
    public static GameContext CreateContext(List<Card64>? hand = null, List<JokerObject>? jokers = null, int startingGold = 10)
    {
        var builder = GameContextBuilder.Create();
        builder.WithDeck(new RedDeckFactory());
        if (hand != null)
            builder.WithHand(hand);
        if (jokers != null)
            foreach (var j in jokers)
                builder.WithJoker(j);

        var ctx = builder.CreateGameContext("TEST_SEED");
        var current = ctx.PersistentState.EconomyHandler.GetCurrentGold();
        if (startingGold > current)
            ctx.PersistentState.EconomyHandler.AddGold(startingGold - current);
        else if (startingGold < current)
            ctx.PersistentState.EconomyHandler.SpendGold(current - startingGold);
        return ctx;
    }

    public static Card64 MakeCard(uint id, Rank r, Suit s, Enhancement e = Enhancement.None)
        => Card64.Create(id, r, s, e);

    public static JokerObject MakeJoker(GameContext ctx, int staticId, Edition ed = Edition.None)
        => ctx.CoreObjectsFactory.CreateJoker(staticId, ed);

    public static uint GetMaxRuntimeId(GameContext ctx)
    {
        uint max = 0;
        foreach (var c in ctx.Hand.Span)
            if (c.Id > max) max = c.Id;
        foreach (var j in ctx.JokerContainer.Jokers)
            if (j.Id > max) max = j.Id;
        return max;
    }
}

[TestFixture]
public class SpectralCardsTests
{
    [Test]
    public void Familiar_RemovesOneCardAndAddsThreeFaceCards()
    {
        var hand = new List<Card64>
        {
            SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade),
            SpectralTestHelper.MakeCard(0, Rank.Three, Suit.Heart)
        };
        var ctx = SpectralTestHelper.CreateContext(hand);
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var fam = new Familiar(100, 0);
        fam.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.Hand.Count, Is.EqualTo(4));
        var newCards = ctx.Hand.List.Where(c => c.Id > prevMax).ToList();
        Assert.That(newCards, Has.Count.EqualTo(3));
        foreach (var c in newCards)
        {
            Assert.That(c.GetRank(), Is.AnyOf(Rank.Jack, Rank.Queen, Rank.King));
            Assert.That(c.GetEnh(), Is.Not.EqualTo(Enhancement.None));
        }
    }

    [Test]
    public void Grim_RemovesOneCardAndAddsTwoAces()
    {
        var hand = new List<Card64>
        {
            SpectralTestHelper.MakeCard(0, Rank.Four, Suit.Spade),
            SpectralTestHelper.MakeCard(0, Rank.Six, Suit.Heart)
        };
        var ctx = SpectralTestHelper.CreateContext(hand);
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var g = new Grim(101, 0);
        g.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.Hand.Count, Is.EqualTo(3));
        var newCards = ctx.Hand.List.Where(c => c.Id > prevMax).ToList();
        Assert.That(newCards, Has.Count.EqualTo(2));
        foreach (var c in newCards)
        {
            Assert.That(c.GetRank(), Is.EqualTo(Rank.Ace));
            Assert.That(c.GetEnh(), Is.Not.EqualTo(Enhancement.None));
        }
    }

    [Test]
    public void Incantation_RemovesOneCardAndAddsFourNumbered()
    {
        var hand = new List<Card64>
        {
            SpectralTestHelper.MakeCard(0, Rank.Five, Suit.Spade)
        };
        var ctx = SpectralTestHelper.CreateContext(hand);
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var inc = new Incantation(102, 0);
        inc.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.Hand.Count, Is.EqualTo(4));
        var newCards = ctx.Hand.List.Where(c => c.Id > prevMax).ToList();
        Assert.That(newCards, Has.Count.EqualTo(4));
        foreach (var c in newCards)
        {
            Assert.That(c.GetRank(), Is.InRange(Rank.Two, Rank.Ten));
            Assert.That(c.GetEnh(), Is.Not.EqualTo(Enhancement.None));
        }
    }

    [Test]
    public void Talisman_SetsGoldSeal()
    {
        var card = SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade);
        var ctx = SpectralTestHelper.CreateContext(new List<Card64> { card });
        var t = new Talisman(103, 0);
        Assert.That(t.IsUsable(ctx, new[] { 0 }));
        t.Apply(ctx, new[] { 0 });
        Assert.That(ctx.Hand.Span[0].GetSeal(), Is.EqualTo(Seal.Gold));
    }

    [Test]
    public void Aura_AddsEdition()
    {
        var card = SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade);
        var ctx = SpectralTestHelper.CreateContext(new List<Card64> { card });
        var a = new Aura(104, 0);
        Assert.That(a.IsUsable(ctx, new[] { 0 }));
        a.Apply(ctx, new[] { 0 });
        Assert.That(ctx.Hand.Span[0].GetEdition(), Is.Not.EqualTo(Edition.None));
    }

    [Test]
    [Ignore("No Rare joker yet")]
    public void Wraith_AddsRareJokerAndRemovesGold()
    {
        var ctx = SpectralTestHelper.CreateContext(startingGold: 10);
        ctx.JokerContainer.Slots = 2;
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var w = new Wraith(105, 0);
        w.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.PersistentState.EconomyHandler.GetCurrentGold(), Is.EqualTo(0));
        var newJokers = ctx.JokerContainer.Jokers.Where(j => j.Id > prevMax).ToList();
        Assert.That(newJokers, Has.Count.EqualTo(1));
        var jokerAttr = JokerRegistry.GetAttribute(newJokers[0].StaticId);
        Assert.That(jokerAttr.Rarity, Is.EqualTo(JokerRarity.Rare));
    }

    [Test]
    public void Sigil_ConvertsAllToSameSuit()
    {
        var hand = new List<Card64>
        {
            SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade),
            SpectralTestHelper.MakeCard(1, Rank.Four, Suit.Heart)
        };
        var ctx = SpectralTestHelper.CreateContext(hand);
        var s = new Sigil(106, 0);
        s.Apply(ctx, System.Array.Empty<int>());

        var suit = ctx.Hand.Span[0].GetSuit();
        Assert.That(ctx.Hand.List.All(c => c.GetSuit() == suit));
    }

    [Test]
    public void Ouija_ConvertsAllToSameRankAndReducesHandSize()
    {
        var hand = new List<Card64>
        {
            SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade),
            SpectralTestHelper.MakeCard(0, Rank.Four, Suit.Heart)
        };
        var ctx = SpectralTestHelper.CreateContext(hand);
        var sizeBefore = ctx.GetHandSize();
        var o = new Ouija(107, 0);
        o.Apply(ctx, System.Array.Empty<int>());

        var rank = ctx.Hand.Span[0].GetRank();
        Assert.That(ctx.Hand.List.All(c => c.GetRank() == rank));
        Assert.That(ctx.GetHandSize(), Is.EqualTo(sizeBefore - 1));
    }

    [Test]
    public void Ectoplasm_SetsJokerNegativeAndDecreasesHandSize()
    {
        var ctx = SpectralTestHelper.CreateContext();
        var j1 = SpectralTestHelper.MakeJoker(ctx, 1);
        var j2 = SpectralTestHelper.MakeJoker(ctx, 1, Edition.Negative);
        ctx.JokerContainer.AddJoker(ctx, j1);
        ctx.JokerContainer.AddJoker(ctx, j2);
        var sizeBefore = ctx.GetHandSize();
        var e = new Ectoplasm(108, 0);
        e.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.GetHandSize(), Is.EqualTo(sizeBefore - 1));
        Assert.That(ctx.PersistentState.EctoplasmUsageCount, Is.EqualTo(1));
        Assert.That(ctx.JokerContainer.Jokers.All(j => j.Edition == Edition.Negative));
    }

    [Test]
    public void Immolate_DestroysUpToFiveAndAddsGold()
    {
        var hand = new List<Card64>();
        for (uint i = 0; i < 6; i++)
            hand.Add(SpectralTestHelper.MakeCard(i, Rank.Two, Suit.Spade));
        var ctx = SpectralTestHelper.CreateContext(hand, startingGold: 0);
        var im = new Immolate(109, 0);
        im.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.Hand.Count, Is.EqualTo(1));
        Assert.That(ctx.PersistentState.EconomyHandler.GetCurrentGold(), Is.EqualTo(20));
    }

    [Test]
    public void Ankh_CopiesJoker()
    {
        var ctx = SpectralTestHelper.CreateContext();
        var j = SpectralTestHelper.MakeJoker(ctx, 1, Edition.Negative);
        ctx.JokerContainer.AddJoker(ctx, j);
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var a = new Ankh(110, 0);
        a.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.JokerContainer.Jokers.Count, Is.EqualTo(2));
        var newJoker = ctx.JokerContainer.Jokers.First(x => x.Id > prevMax);
        Assert.That(newJoker.Edition, Is.EqualTo(Edition.None));
        Assert.That(newJoker.StaticId, Is.EqualTo(j.StaticId));
    }

    [Test]
    public void DejaVu_SetsRedSeal()
    {
        var card = SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade);
        var ctx = SpectralTestHelper.CreateContext(new List<Card64> { card });
        var d = new DejaVu(111, 0);
        d.Apply(ctx, new[] { 0 });
        Assert.That(ctx.Hand.Span[0].GetSeal(), Is.EqualTo(Seal.Red));
    }

    [Test]
    public void Hex_AddsPolyToOneAndRemovesOthers()
    {
        var ctx = SpectralTestHelper.CreateContext();
        var j1 = SpectralTestHelper.MakeJoker(ctx, 1);
        var j2 = SpectralTestHelper.MakeJoker(ctx, 1);
        ctx.JokerContainer.AddJoker(ctx, j1);
        ctx.JokerContainer.AddJoker(ctx, j2);
        var h = new Hex(112, 0);
        h.Apply(ctx, System.Array.Empty<int>());

        Assert.That(ctx.JokerContainer.Jokers.Count, Is.EqualTo(1));
        Assert.That(ctx.JokerContainer.Jokers[0].Edition, Is.EqualTo(Edition.Poly));
    }

    [Test]
    public void Trance_SetsBlueSeal()
    {
        var card = SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade);
        var ctx = SpectralTestHelper.CreateContext(new List<Card64> { card });
        var t = new Trance(113, 0);
        t.Apply(ctx, new[] { 0 });
        Assert.That(ctx.Hand.Span[0].GetSeal(), Is.EqualTo(Seal.Blue));
    }

    [Test]
    public void Medium_SetsPurpleSeal()
    {
        var card = SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade);
        var ctx = SpectralTestHelper.CreateContext(new List<Card64> { card });
        var m = new Medium(114, 0);
        m.Apply(ctx, new[] { 0 });
        Assert.That(ctx.Hand.Span[0].GetSeal(), Is.EqualTo(Seal.Purple));
    }

    [Test]
    public void Cryptid_CreatesTwoCopies()
    {
        var card = SpectralTestHelper.MakeCard(0, Rank.Two, Suit.Spade);
        var ctx = SpectralTestHelper.CreateContext(new List<Card64> { card });
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var c = new Cryptid(115, 0);
        c.Apply(ctx, new[] { 0 });

        Assert.That(ctx.Hand.Count, Is.EqualTo(3));
        var newCards = ctx.Hand.List.Where(x => x.Id > prevMax).ToArray();
        Assert.That(newCards, Has.Length.EqualTo(2));
        var raw = ctx.Hand.Span[0].GetRaw();
        Assert.That(newCards[0].GetRaw(), Is.EqualTo(raw));
        Assert.That(newCards[1].GetRaw(), Is.EqualTo(raw));
    }

    [Test]
    [Ignore("No legendary joker yet")]
    public void TheSoul_AddsLegendaryJoker()
    {
        var ctx = SpectralTestHelper.CreateContext();
        ctx.JokerContainer.Slots = 2;
        var prevMax = SpectralTestHelper.GetMaxRuntimeId(ctx);
        var s = new TheSoul(116, 0);
        s.Apply(ctx, System.Array.Empty<int>());

        var newJokers = ctx.JokerContainer.Jokers.Where(j => j.Id > prevMax).ToList();
        Assert.That(newJokers, Has.Count.EqualTo(1));
        var attr = JokerRegistry.GetAttribute(newJokers[0].StaticId);
        Assert.That(attr.Rarity, Is.EqualTo(JokerRarity.Legendary));
    }

    [Test]
    public void BlackHole_UpgradesAllHands()
    {
        var ctx = SpectralTestHelper.CreateContext();
        var levelsBefore = new Dictionary<HandRank, int>();
        foreach (HandRank hr in Enum.GetValues<HandRank>())
            levelsBefore[hr] = ctx.PersistentState.HandTracker.GetHandLevel(hr);
        var b = new BlackHole(117, 0);
        b.Apply(ctx, System.Array.Empty<int>());

        foreach (HandRank hr in Enum.GetValues<HandRank>())
            Assert.That(ctx.PersistentState.HandTracker.GetHandLevel(hr), Is.EqualTo(levelsBefore[hr] + 1));
    }
}