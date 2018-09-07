using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;

namespace BitcoinVanityAddressFinder.Services
{
    public class VanityAddressResult
    {
        public int AttemptCount { get; set; }
        public Key PrivateKey { get; set; }
    }

    public class VanityAddressService
    {
        private int _attemptCount;

        public Task<VanityAddressResult> Search(
            int cores,
            string vanityText,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith,
            Network network,
            CancellationToken ct)
        {
            return Task.Factory.StartNew(() =>
            {
                var tasks = new List<Task<Key>>();

                for (int i = 0; i < cores; i++)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        string address = "";
                        Key privateKey = null;

                        while (!IsVanityAddress(address, vanityText, isCaseSensitive, isStartsWith, isEndsWith))
                        {
                            if (ct.IsCancellationRequested)
                            {
                                ct.ThrowIfCancellationRequested();
                            }

                            privateKey = new Key();

                            address = privateKey.PubKey.GetAddress(network).ToString();

                            Interlocked.Increment(ref _attemptCount);
                        }

                        return privateKey;
                    }, ct));
                }

                var resultResult = Task.WhenAny(tasks.ToArray()).Result.Result;
                return new VanityAddressResult { AttemptCount = _attemptCount, PrivateKey = resultResult };
            }, ct);
        }

        public static bool IsVanityAddress(
            string address,
            string vanityText,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith)
        {
            // TODO - Get the actual length
            if (address.Length < 3)
            {
                return false;
            }

            if (isCaseSensitive)
            {
                if (isStartsWith && isEndsWith)
                {
                    return address.Remove(0, 1).StartsWith(vanityText) && address.EndsWith(vanityText);
                }

                if (isStartsWith)
                {
                    return address.Remove(0, 1).StartsWith(vanityText);
                }
                if (isEndsWith)
                {
                    return address.EndsWith(vanityText);
                }
                else
                {
                    return address.Contains(vanityText);
                }
            }
            else
            {
                if (isStartsWith && isEndsWith)
                {
                    return address.Remove(0, 1).ToUpper().StartsWith(vanityText.ToUpper()) && address.ToUpper().EndsWith(vanityText.ToUpper());
                }

                else if (isStartsWith)
                {
                    return address.Remove(0, 1).ToUpper().StartsWith(vanityText.ToUpper());
                }
                else if (isEndsWith)
                {
                    return address.ToUpper().EndsWith(vanityText.ToUpper());
                }
                else
                {
                    return address.ToUpper().Contains(vanityText.ToUpper());
                }
            }
        }
    }
}