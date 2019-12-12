using System;

namespace BitcoinVanityAddressFinder.Services
{
    public class InputStringVerifierService
    {
        private readonly object _lock = new object();
        private readonly string _vanityText;
        private readonly bool _isCaseSensitive;
        private readonly bool _isStartsWith;
        private readonly bool _isEndsWith;

        public InputStringVerifierService(
            string vanityText,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith)
        {
            _vanityText = vanityText;
            _isCaseSensitive = isCaseSensitive;
            _isStartsWith = isStartsWith;
            _isEndsWith = isEndsWith;
        }

        public bool IsVanityAddress(string address)
        {
            lock (_lock)
            {
                // TODO - Get the actual length
                if (address.Length < 3)
                {
                    return false;
                }

                if (_isCaseSensitive)
                {
                    if (_isStartsWith && _isEndsWith)
                    {
                        return address.Remove(0, 1).StartsWith(_vanityText, StringComparison.InvariantCulture) && address.EndsWith(_vanityText, StringComparison.InvariantCulture);
                    }

                    if (_isStartsWith)
                    {
                        return address.Remove(0, 1).StartsWith(_vanityText, StringComparison.InvariantCulture);
                    }

                    if (_isEndsWith)
                    {
                        return address.EndsWith(_vanityText, StringComparison.InvariantCulture);
                    }

                    return address.Contains(_vanityText);
                }

                if (_isStartsWith && _isEndsWith)
                {
                    return address.Remove(0, 1).StartsWith(_vanityText, StringComparison.InvariantCultureIgnoreCase) && address.EndsWith(_vanityText, StringComparison.InvariantCultureIgnoreCase);
                }

                if (_isStartsWith)
                {
                    return address.Remove(0, 1).StartsWith(_vanityText, StringComparison.InvariantCultureIgnoreCase);
                }

                if (_isEndsWith)
                {
                    return address.EndsWith(_vanityText, StringComparison.InvariantCultureIgnoreCase);
                }

                return address.ToUpper().Contains(_vanityText.ToUpper());
            }
        }
    }
}