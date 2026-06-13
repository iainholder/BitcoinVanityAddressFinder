using BitcoinVanityAddressFinder.Services;
using BitcoinVanityAddressFinder.ViewModel;
using NUnit.Framework;

namespace BitcoinVanityAddressFinder.Tests
{
    [TestFixture]
    public class VanityTextValidationTests
    {
        private static VanityAddressViewModel CreateViewModel() => new(new ServiceFactory());

        [TestCase("0")] // zero
        [TestCase("O")] // capital o
        [TestCase("I")] // capital i
        [TestCase("l")] // lower L
        [TestCase("good0")]
        public void VanityText_WithImpossibleBase58Char_IsInvalid_WhenCaseSensitive(string text)
        {
            var vm = CreateViewModel();
            vm.IsCaseSensitive = true;
            vm.VanityText = text;

            Assert.That(vm["VanityText"], Does.StartWith("A Bitcoin address can never contain"));
        }

        [TestCase("Love")]
        [TestCase("abc")]
        [TestCase("Satoshi")]
        public void VanityText_WithValidBase58_IsValid(string text)
        {
            var vm = CreateViewModel();
            vm.VanityText = text;

            Assert.That(vm["VanityText"], Is.EqualTo(""));
        }

        [Test]
        public void VanityText_CaseInsensitive_AllowsCharsWithAValidOppositeCase()
        {
            var vm = CreateViewModel();
            vm.IsCaseSensitive = false;

            // O/o, I/i, l/L each have a valid Base58 counterpart, so they can still match.
            vm.VanityText = "OIl";
            Assert.That(vm["VanityText"], Is.EqualTo(""));
        }

        [Test]
        public void VanityText_CaseInsensitive_StillRejectsZero()
        {
            var vm = CreateViewModel();
            vm.IsCaseSensitive = false;

            vm.VanityText = "0";
            Assert.That(vm["VanityText"], Does.Contain("0"));
        }

        [Test]
        public void VanityText_NonAlphanumeric_IsInvalid()
        {
            var vm = CreateViewModel();
            vm.VanityText = "ab!";
            Assert.That(vm["VanityText"], Is.EqualTo("Letters and numbers only"));
        }

        [Test]
        public void VanityText_TooLong_IsInvalid()
        {
            var vm = CreateViewModel();
            vm.VanityText = "abcdefgh"; // 8 characters
            Assert.That(vm["VanityText"], Is.EqualTo("That would take too long"));
        }

        [Test]
        public void VanityText_Empty_ShowsNoError()
        {
            var vm = CreateViewModel();
            vm.VanityText = "";
            Assert.That(vm["VanityText"], Is.EqualTo(""));
        }
    }
}
