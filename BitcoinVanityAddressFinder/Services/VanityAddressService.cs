using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using BitcoinVanityAddressFinder.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using NBitcoin;

namespace BitcoinVanityAddressFinder.Services
{
    public class AttemptCountMessage
    {
        public int AttemptCount { get; set; }

        public string AttemptCountMessageTokenGuid { get; set; }
    }

    public class VanityAddressService : IDisposable
    {
        private int _attemptCount;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Task<Key> Search(
            int cores,
            SearchMode searchMode,
            string vanityText,
            int minWordLength,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith,
            Network network,
            string attemptCountMessageTokenGuid,
            CancellationToken ct)
        {
            _attemptCount = 0;

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (sender, args) => SendAttemptCountMessage(attemptCountMessageTokenGuid);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            return Task.Run(() =>
            {
                var tasks = new List<Task<Key>>();

                for (int i = 0; i < cores; i++)
                {
                    tasks.Add(Task.Run(() =>
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
                            var words = GetWordsHashSet(minWordLength);

                            while (!IsDictionaryWordAddress(address, words, isCaseSensitive, isStartsWith, isEndsWith))
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

                dispatcherTimer.Stop();

                SendAttemptCountMessage(attemptCountMessageTokenGuid);

                return resultResult;
            }, ct);
        }

        private void SendAttemptCountMessage(string attemptCountMessageTokenGuid)
        {
            Messenger.Default.Send(new AttemptCountMessage { AttemptCount = _attemptCount, AttemptCountMessageTokenGuid = attemptCountMessageTokenGuid });
        }

        private static HashSet<string> GetWordsHashSet(int minWordLength)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string dictionaryTxt = "BitcoinVanityAddressFinder.Services.Dictionary.txt";

            using (var stream = assembly.GetManifestResourceStream(dictionaryTxt))
            {
                // TODO - Improve handling of this exception
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException("Dictionary not found.")))
                {
                    var result = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    return result
                        .Where(o => o.Length >= minWordLength)
                        .Distinct()
                        .ToHashSet();
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

                return address.Contains(vanityText);
            }

            if (isStartsWith && isEndsWith)
            {
                return address.Remove(0, 1).ToUpper().StartsWith(vanityText.ToUpper()) && address.ToUpper().EndsWith(vanityText.ToUpper());
            }

            if (isStartsWith)
            {
                return address.Remove(0, 1).ToUpper().StartsWith(vanityText.ToUpper());
            }

            if (isEndsWith)
            {
                return address.ToUpper().EndsWith(vanityText.ToUpper());
            }

            return address.ToUpper().Contains(vanityText.ToUpper());
        }

        public static bool IsDictionaryWordAddress(
            string address,
            HashSet<string> words,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith)
        {
            if (address.Length < 3)
            {
                return false;
            }

            if (isCaseSensitive)
            {
                if (isStartsWith && isEndsWith)
                {
                    return words.Any(o => address.Remove(0, 1).StartsWith(o)) && words.Any(o => address.Remove(0, 1).EndsWith(o));
                }

                if (isStartsWith)
                {
                    return words.Any(o => address.Remove(0, 1).StartsWith(o));
                }

                if (isEndsWith)
                {
                    return words.Any(address.EndsWith);
                }

                return words.Any(address.Contains);
            }

            if (isStartsWith && isEndsWith)
            {
                return words.Any(o => address.Remove(0, 1).ToUpper().StartsWith(o.ToUpper())) && words.Any(o => address.Remove(0, 1).ToUpper().EndsWith(o.ToUpper()));
            }

            if (isStartsWith)
            {
                return words.Any(o => address.Remove(0, 1).ToUpper().StartsWith(o.ToUpper()));
            }

            if (isEndsWith)
            {
                return words.Any(o => address.ToUpper().EndsWith(o.ToUpper()));
            }

            return words.Any(o => address.ToUpper().Contains(o.ToUpper()));
        }
    }
}