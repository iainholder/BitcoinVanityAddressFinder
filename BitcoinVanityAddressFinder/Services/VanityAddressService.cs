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
    public class VanityAddressService : IDisposable
    {
        private static SemaphoreSlim _semaphore;
        private int _attemptCount;

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
            _semaphore = new SemaphoreSlim(cores, cores);

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += (sender, args) => Messenger.Default.Send(_attemptCount, attemptCountMessageTokenGuid);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            return Task.Run(() =>
            {
                var tasks = new List<Task<Key>>();

                for (int i = 0; i < cores; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        _semaphore.Wait(ct);

                        string address = "";
                        Key privateKey = null;

                        if (searchMode == SearchMode.String)
                        {
                            var inputStringVerifier = new InputStringVerifier(vanityText, isCaseSensitive, isStartsWith, isEndsWith);

                            while (!inputStringVerifier.IsVanityAddress(address))
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    ct.ThrowIfCancellationRequested();
                                }

                                privateKey = new Key();

                                address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network).ToString();

                                Interlocked.Increment(ref _attemptCount);
                            }
                        }

                        if (searchMode == SearchMode.Dictionary)
                        {
                            // Getting a new set of words for each thread is probably overkill as reading HashSet should be thread safe.
                            // Options are:
                            // 1. Leave it in.
                            // 2. Use single hash set to save a little memory.
                            // 3. Use immutable hashset for confirmed thread safety and 2.
                            var words = GetWordsHashSet(minWordLength);

                            var dictionaryWordVerifier = new DictionaryWordVerifier(words, isCaseSensitive, isStartsWith, isEndsWith);

                            while (!dictionaryWordVerifier.IsDictionaryWordAddress(address))
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    ct.ThrowIfCancellationRequested();
                                }

                                privateKey = new Key();

                                address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network).ToString();

                                Interlocked.Increment(ref _attemptCount);
                            }
                        }

                        _semaphore.Release();

                        return privateKey;
                    }, ct));
                }

                var resultResult = Task.WhenAny(tasks.ToArray()).Result.Result;

                dispatcherTimer.Stop();

                Messenger.Default.Send(_attemptCount, attemptCountMessageTokenGuid);

                return resultResult;
            }, ct);
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
                    var words = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    return words
                        .Where(o => o.Length >= minWordLength)
                        .Distinct()
                        .ToHashSet();
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}