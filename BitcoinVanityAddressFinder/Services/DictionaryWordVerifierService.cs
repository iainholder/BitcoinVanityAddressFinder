using System;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinVanityAddressFinder.Services
{
    public class DictionaryWordVerifierService
    {
        private readonly object _lock = new object();
        private readonly HashSet<string> _words;
        private readonly bool _isCaseSensitive;
        private readonly bool _isStartsWith;
        private readonly bool _isEndsWith;

        public DictionaryWordVerifierService(
            HashSet<string> words,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith)
        {
            _words = words;
            _isCaseSensitive = isCaseSensitive;
            _isStartsWith = isStartsWith;
            _isEndsWith = isEndsWith;
        }

        public bool IsDictionaryWordAddress(string address)
        {
            // TODO - Is this lock required? Probably not. Lockless thread-safe code but the cost is minimal compared to rest so leave in for now.
            lock (_lock)
            {
                if (address.Length < 3)
                {
                    return false;
                }

                if (_isCaseSensitive)
                {
                    if (_isStartsWith && _isEndsWith)
                    {
                        return _words.Any(o => address.Remove(0, 1).StartsWith(o, StringComparison.InvariantCulture)) 
                               && _words.Any(o => address.Remove(0, 1).EndsWith(o, StringComparison.InvariantCulture));
                    }

                    if (_isStartsWith)
                    {
                        return _words.Any(o => address.Remove(0, 1).StartsWith(o, StringComparison.InvariantCulture));
                    }

                    if (_isEndsWith)
                    {
                        return _words.Any(address.EndsWith);
                    }

                    return _words.Any(address.Contains);
                }

                if (_isStartsWith && _isEndsWith)
                {
                    return _words.Any(o => address.Remove(0, 1).StartsWith(o, StringComparison.InvariantCultureIgnoreCase)) 
                           && _words.Any(o => address.Remove(0, 1).EndsWith(o, StringComparison.InvariantCultureIgnoreCase));
                }

                if (_isStartsWith)
                {
                    return _words.Any(o => address.Remove(0, 1).StartsWith(o, StringComparison.InvariantCultureIgnoreCase));
                }

                if (_isEndsWith)
                {
                    return _words.Any(o => address.EndsWith(o, StringComparison.InvariantCultureIgnoreCase));
                }

                return _words.Any(o => address.ToUpper().Contains(o.ToUpper()));
            }
        }
    }
}