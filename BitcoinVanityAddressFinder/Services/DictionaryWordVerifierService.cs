using System;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinVanityAddressFinder.Services
{
    public class DictionaryWordVerifierService
    {
        // No locking: each worker thread gets its own verifier instance and only ever reads
        // the (shared, never-mutated) word set, so this runs lock-free on the hot path.
        private readonly HashSet<string> _words;
        private readonly bool _isStartsWith;
        private readonly bool _isEndsWith;
        private readonly StringComparison _comparison;

        public DictionaryWordVerifierService(
            HashSet<string> words,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith)
        {
            _words = words;
            _isStartsWith = isStartsWith;
            _isEndsWith = isEndsWith;
            _comparison = isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public bool IsDictionaryWordAddress(string address)
        {
            if (address.Length < 3)
            {
                return false;
            }

            // The first character of a Bitcoin address is a fixed network/type prefix, so
            // "starts with" matching ignores it. Computed once rather than per word.
            string addressWithoutPrefix = address[1..];

            if (_isStartsWith && _isEndsWith)
            {
                return _words.Any(o => addressWithoutPrefix.StartsWith(o, _comparison))
                       && _words.Any(o => address.EndsWith(o, _comparison));
            }

            if (_isStartsWith)
            {
                return _words.Any(o => addressWithoutPrefix.StartsWith(o, _comparison));
            }

            if (_isEndsWith)
            {
                return _words.Any(o => address.EndsWith(o, _comparison));
            }

            return _words.Any(o => address.Contains(o, _comparison));
        }
    }
}
