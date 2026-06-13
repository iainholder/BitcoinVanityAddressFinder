using System;

namespace BitcoinVanityAddressFinder.Services
{
    public class InputStringVerifierService(
        string vanityText,
        bool isCaseSensitive,
        bool isStartsWith,
        bool isEndsWith)
    {
        // No locking: each worker thread gets its own verifier instance and never shares it,
        // so this runs lock-free on the hot path (called once per generated key).
        private readonly StringComparison _comparison = isCaseSensitive
            ? StringComparison.InvariantCulture
            : StringComparison.InvariantCultureIgnoreCase;

        public bool IsVanityAddress(string address)
        {
            if (address.Length < 3)
            {
                return false;
            }

            // The first character of a Bitcoin address is a fixed network/type prefix, so
            // "starts with" matching ignores it.
            string addressWithoutPrefix = address[1..];

            bool matchesStart = !isStartsWith || addressWithoutPrefix.StartsWith(vanityText, _comparison);
            bool matchesEnd = !isEndsWith || address.EndsWith(vanityText, _comparison);

            if (isStartsWith || isEndsWith)
            {
                return matchesStart && matchesEnd;
            }

            return address.Contains(vanityText, _comparison);
        }
    }
}
