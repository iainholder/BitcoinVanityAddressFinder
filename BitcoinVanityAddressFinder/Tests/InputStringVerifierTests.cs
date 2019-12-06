using BitcoinVanityAddressFinder.Services;
using NUnit.Framework;

namespace BitcoinVanityAddressFinder.Tests
{
    [TestFixture]
    public class InputStringVerifierTests
    {
        [TestCase("1111111111111111111111111111111111", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXX1")]
        public void IsVanityAddress_WhenNotContains_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, false, false, false);
            Assert.IsFalse(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1111111111111111111111111111111111", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "XXX1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "XXX1")]
        public void IsVanityAddress_WhenContains_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, false, false, false);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xXX1")]
        public void IsVanityAddress_WhenNotCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, false, false, false);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xXX1")]
        public void IsVanityAddress_WhenCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, false, false);
            Assert.False(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1XXXX11111111111111111111111111111", "XXXX")]
        [TestCase("1XXXX11111111111111111111111111111", "xxxx")]
        public void IsVanityAddress_WhenStartsWith_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, false, true, false);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("11111111111111111XXXX1111111111111", "XXXX")]
        [TestCase("111111111111111111111111111111xxxx", "xxxx")]
        public void IsVanityAddress_WhenStartsWith_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, false, true, false);
            Assert.IsFalse(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1XXXX1111111111111111111111111XXXX", "XXXX")]
        [TestCase("1XXXX1111111111111111111111111XXXX", "xxxx")]
        public void IsVanityAddress_WhenEndsWith_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, false, false, true);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1111111111111111111111111111111111", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "XXX1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "XXX1")]
        public void IsVanityAddress_WhenContainsAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, false, false);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xxx1")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxx1xxxxxxxxxxxx", "XXX1")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx1", "XXX1")]
        public void IsVanityAddress_WhenContainsAndCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, false, false);
            Assert.IsFalse(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1XXXX11111111111111111111111111111", "XXXX")]
        [TestCase("1xxxx11111111111111111111111111111", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, true, false);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1xxxx11111111111111111111111111111", "XXXX")]
        [TestCase("1XXXX11111111111111111111111111111", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, true, false);
            Assert.IsFalse(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("111111111111111111111111111111XXXX", "XXXX")]
        [TestCase("111111111111111111111111111111xxxx", "xxxx")]
        public void IsVanityAddress_WhenEndsWithAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, false, true);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("111111111111111111111111111111xxxx", "XXXX")]
        [TestCase("1XXXX1111111111111111111111111XXXX", "xxxx")]
        public void IsVanityAddress_WhenEndsWithAndCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, false, true);
            Assert.IsFalse(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1XXXX1111111111111111111111111XXXX", "XXXX")]
        [TestCase("1xxxx1111111111111111111111111xxxx", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndEndsWithAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, true, true);
            Assert.IsTrue(vanityAddressVerifier.IsVanityAddress(address));
        }

        [TestCase("1XXXX1111111111111111111111111xxxx", "XXXX")]
        [TestCase("1xxxx1111111111111111111111111XXXX", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndEndsWithAndCaseSensitive_DontMatch(string address,
            string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifier(vanityText, true, true, true);
            Assert.IsFalse(vanityAddressVerifier.IsVanityAddress(address));
        }
    }
}