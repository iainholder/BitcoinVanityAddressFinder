using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BitcoinVanityAddressFinder.ViewModel;
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
            SearchMode searchMode,
            string vanityText,
            int minWordLength,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith,
            Network network,
            CancellationToken ct)
        {
            return Task.Factory.StartNew(() =>
            {
                _attemptCount = 0;

                var tasks = new List<Task<Key>>();

                for (int i = 0; i < cores; i++)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        string address = "";
                        Key privateKey = null;

                        if (searchMode == SearchMode.String)
                        {
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
                        }

                        if (searchMode == SearchMode.Dictionary)
                        {
                            // TODO - Compile this in
                            Dictionary<string, string> dictionary = GetDictionary(minWordLength);

                            string mainNetAddress = "";

                            while (!IsDictionaryWordAddress(address, dictionary, isCaseSensitive, isStartsWith, isEndsWith))
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    ct.ThrowIfCancellationRequested();
                                }

                                privateKey = new Key();

                                address = privateKey.PubKey.GetAddress(network).ToString();

                                Interlocked.Increment(ref _attemptCount);
                            }
                        }

                        return privateKey;
                    }, ct));
                }

                var resultResult = Task.WhenAny(tasks.ToArray()).Result.Result;
                return new VanityAddressResult { AttemptCount = _attemptCount, PrivateKey = resultResult };
            }, ct);
        }

        private Dictionary<string, string> GetDictionary(int minWordLength)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dictionaryTxt = "BitcoinVanityAddressFinder.Services.Dictionary.txt";

            using (var stream = assembly.GetManifestResourceStream(dictionaryTxt))
            {
                // TODO - Improve handling of this exception
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException("Dictionary not found.")))
                {
                    var result = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    return result
                        .Where(o => o.Length >= minWordLength)
                        .ToDictionary(o => o);
                    ;
                }
            }
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

        public static bool IsDictionaryWordAddress(
            string address,
            Dictionary<string, string> dictionary,
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
                    return dictionary.Any(o => address.Remove(0, 1).StartsWith(o.Key)) && dictionary.Any(o => address.Remove(0, 1).EndsWith(o.Key));
                    //return address.Remove(0, 1).StartsWith(vanityText) && address.EndsWith(vanityText);
                }

                if (isStartsWith)
                {
                    return dictionary.Any(o => address.Remove(0, 1).StartsWith(o.Key));
                    // return address.Remove(0, 1).StartsWith(vanityText);
                }

                if (isEndsWith)
                {
                    return dictionary.Any(o => address.EndsWith(o.Key));
                    // return address.EndsWith(vanityText);
                }
                else
                {
                    return dictionary.Any(o => address.Contains(o.Key));
                    // return address.Contains(vanityText);
                }
            }
            else
            {
                if (isStartsWith && isEndsWith)
                {
                    return dictionary.Any(o => address.Remove(0, 1).ToUpper().StartsWith(o.Key.ToUpper())) && dictionary.Any(o => address.Remove(0, 1).ToUpper().EndsWith(o.Key.ToUpper()));
                    // return address.Remove(0, 1).ToUpper().StartsWith(vanityText.ToUpper()) && address.ToUpper().EndsWith(vanityText.ToUpper());
                }

                if (isStartsWith)
                {
                    return dictionary.Any(o => address.Remove(0, 1).ToUpper().StartsWith(o.Key.ToUpper()));
                    // return address.Remove(0, 1).ToUpper().StartsWith(vanityText.ToUpper());
                }

                if (isEndsWith)
                {
                    return dictionary.Any(o => address.ToUpper().EndsWith(o.Key.ToUpper()));
                    //return address.ToUpper().EndsWith(vanityText.ToUpper());
                }
                else
                {
                    return dictionary.Any(o => address.ToUpper().Contains(o.Key.ToUpper()));
                    // return address.ToUpper().Contains(vanityText.ToUpper());
                }
            }
        }
    }
}