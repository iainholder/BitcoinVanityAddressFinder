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
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, false, false, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }

        [TestCase("1111111111111111111111111111111111", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "XXX1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "XXX1")]
        public void IsVanityAddress_WhenContains_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, false, false, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xXX1")]
        public void IsVanityAddress_WhenNotCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, false, false, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xXX1")]
        public void IsVanityAddress_WhenCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, false, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }

        [TestCase("1XXXX11111111111111111111111111111", "XXXX")]
        [TestCase("1XXXX11111111111111111111111111111", "xxxx")]
        public void IsVanityAddress_WhenStartsWith_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, false, true, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("11111111111111111XXXX1111111111111", "XXXX")]
        [TestCase("111111111111111111111111111111xxxx", "xxxx")]
        public void IsVanityAddress_WhenStartsWith_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, false, true, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }

        [TestCase("1XXXX1111111111111111111111111XXXX", "XXXX")]
        [TestCase("1XXXX1111111111111111111111111XXXX", "xxxx")]
        public void IsVanityAddress_WhenEndsWith_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, false, false, true);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("1111111111111111111111111111111111", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "XXX1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "XXX1")]
        public void IsVanityAddress_WhenContainsAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, false, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xxx1")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxx1xxxxxxxxxxxx", "XXX1")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx1", "XXX1")]
        public void IsVanityAddress_WhenContainsAndCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, false, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }

        [TestCase("1XXXX11111111111111111111111111111", "XXXX")]
        [TestCase("1xxxx11111111111111111111111111111", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, true, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("1xxxx11111111111111111111111111111", "XXXX")]
        [TestCase("1XXXX11111111111111111111111111111", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, true, false);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }

        [TestCase("111111111111111111111111111111XXXX", "XXXX")]
        [TestCase("111111111111111111111111111111xxxx", "xxxx")]
        public void IsVanityAddress_WhenEndsWithAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, false, true);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("111111111111111111111111111111xxxx", "XXXX")]
        [TestCase("1XXXX1111111111111111111111111XXXX", "xxxx")]
        public void IsVanityAddress_WhenEndsWithAndCaseSensitive_DontMatch(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, false, true);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }

        [TestCase("1XXXX1111111111111111111111111XXXX", "XXXX")]
        [TestCase("1xxxx1111111111111111111111111xxxx", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndEndsWithAndCaseSensitive_Match(string address, string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, true, true);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.True);
        }

        [TestCase("1XXXX1111111111111111111111111xxxx", "XXXX")]
        [TestCase("1xxxx1111111111111111111111111XXXX", "xxxx")]
        public void IsVanityAddress_WhenStartsWithAndEndsWithAndCaseSensitive_DontMatch(string address,
            string vanityText)
        {
            var vanityAddressVerifier = new InputStringVerifierService(vanityText, true, true, true);
            Assert.That(vanityAddressVerifier.IsVanityAddress(address), Is.False);
        }
    }
}