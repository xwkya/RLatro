using Balatro.Core.GameEngine.PseudoRng;

namespace RLatro.Test.GameEngine
{
    [TestFixture]
    public sealed class LuaRandomTests
    {
        /// <summary>
        /// Random numbers generated in lua: 84, 89, 12, 46, 76
        /// </summary>
        /// <code>
        /// math.randomseed(15)
        /// math.random(1, 100)
        /// ..
        /// math.random(1, 100)
        /// </code>
        [Test]
        public void TestRngWithSetSeed()
        {
            var rng = new LuaRandom(15);   // second arg defaults to 0
            Assert.That(rng.Next(1, 100), Is.EqualTo(84));
            Assert.That(rng.Next(1, 100), Is.EqualTo(89));
            Assert.That(rng.Next(1, 100), Is.EqualTo(12));
            Assert.That(rng.Next(1, 100), Is.EqualTo(46));
            Assert.That(rng.Next(1, 100), Is.EqualTo(76));
        }
    }
}