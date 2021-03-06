﻿using System;
using System.Collections.Generic;
using System.Linq;
using BitcoinVanityAddressFinder.Services;
using NUnit.Framework;

namespace BitcoinVanityAddressFinder.Tests
{
    [TestFixture]
    public class DictionaryWordVerifierTests
    {
        [TestCase("1111111111111111111111111111111111", "XXXX;YYYY;ZZZZ")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXX1")]
        public void IsDictionaryWordAddress_WhenNotContains_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, false, false);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1111111111111111111111111111111111", "1111;2222;3333")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXX;YYYY;ZZZZ")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "XXX1;YYY2;ZZZ2")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "XXX1;YYY2;ZZZ2")]
        public void IsDictionaryWordAddress_WhenContains_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, false, false);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xXX1")]
        public void IsDictionaryWordAddress_WhenNotCaseSensitive_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, false, false);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xXX1")]
        public void IsDictionaryWordAddress_WhenCaseSensitive_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, false, false);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1XXXX11111111111111111111111111111", "XXXX")]
        [TestCase("1XXXX11111111111111111111111111111", "xxxx")]
        public void IsDictionaryWordAddress_WhenStartsWith_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, true, false);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("11111111111111111XXXX1111111111111", "XXXX")]
        [TestCase("111111111111111111111111111111xxxx", "xxxx")]
        public void IsDictionaryWordAddress_WhenStartsWith_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, true, false);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1XXXX1111111111111111111111111XXXX", "XXXX")]
        [TestCase("1XXXX1111111111111111111111111XXXX", "xxxx")]
        public void IsDictionaryWordAddress_WhenEndsWith_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, false, true);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1111111111111111111111111111111111", "1111")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXX")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "XXX1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "XXX1")]
        public void IsDictionaryWordAddress_WhenContainsAndCaseSensitive_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), false, false, false);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "xxxx")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXXX", "xxx1")]
        [TestCase("XXXXXXXXXXXXXXXXXXXXX1XXXXXXXXXXX1", "xxx1")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "XXXX")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxx1xxxxxxxxxxxx", "XXX1")]
        [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx1", "XXX1")]
        public void IsDictionaryWordAddress_WhenContainsAndCaseSensitive_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, false, false);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1XXXX11111111111111111111111111111", "XXXX")]
        [TestCase("1xxxx11111111111111111111111111111", "xxxx")]
        public void IsDictionaryWordAddress_WhenStartsWithAndCaseSensitive_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, true, false);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1xxxx11111111111111111111111111111", "XXXX")]
        [TestCase("1XXXX11111111111111111111111111111", "xxxx")]
        public void IsDictionaryWordAddress_WhenStartsWithAndCaseSensitive_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, true, false);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("111111111111111111111111111111XXXX", "XXXX")]
        [TestCase("111111111111111111111111111111xxxx", "xxxx")]
        public void IsDictionaryWordAddress_WhenEndsWithAndCaseSensitive_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, false, true);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("111111111111111111111111111111xxxx", "XXXX")]
        [TestCase("1XXXX1111111111111111111111111XXXX", "xxxx")]
        public void IsDictionaryWordAddress_WhenEndsWithAndCaseSensitive_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, false, true);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1XXXX1111111111111111111111111XXXX", "XXXX")]
        [TestCase("1xxxx1111111111111111111111111xxxx", "xxxx")]
        public void IsDictionaryWordAddress_WhenStartsWithAndEndsWithAndCaseSensitive_Match(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, true, true);
            Assert.IsTrue(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        [TestCase("1XXXX1111111111111111111111111xxxx", "XXXX")]
        [TestCase("1xxxx1111111111111111111111111XXXX", "xxxx")]
        public void IsDictionaryWordAddress_WhenStartsWithAndEndsWithAndCaseSensitive_DontMatch(string address, string input)
        {
            var dictionaryWordVerifier = new DictionaryWordVerifierService(InputStringToHashSet(input), true, true, true);
            Assert.IsFalse(dictionaryWordVerifier.IsDictionaryWordAddress(address));
        }

        private static HashSet<string> InputStringToHashSet(string input)
        {
            return input.Split(new[] { ';' }, StringSplitOptions.None).ToHashSet();
        }
    }
}