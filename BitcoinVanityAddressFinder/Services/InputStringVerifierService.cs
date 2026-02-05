using System;
using System.Threading;

namespace BitcoinVanityAddressFinder.Services
{
    public class InputStringVerifierService(
        string vanityText,
        bool isCaseSensitive,
        bool isStartsWith,
        bool isEndsWith)
    {
        private readonly Lock _lock = new();

        public bool IsVanityAddress(string address)
        {
            lock (_lock)
            {
                if (address.Length < 3)
                {
                    return false;
                }

                var comparison = isCaseSensitive
                    ? StringComparison.InvariantCulture
                    : StringComparison.InvariantCultureIgnoreCase;

                string addressWithoutPrefix = address[1..];

                bool matchesStart = !isStartsWith || addressWithoutPrefix.StartsWith(vanityText, comparison);
                bool matchesEnd = !isEndsWith || address.EndsWith(vanityText, comparison);

                if (isStartsWith || isEndsWith)
                {
                    return matchesStart && matchesEnd;
                }

                return address.Contains(vanityText, comparison);
            }
        }
    }
}