using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using BitcoinVanityAddressFinder.ViewModel;
using CommunityToolkit.Mvvm.Messaging;
using NBitcoin;

namespace BitcoinVanityAddressFinder.Services
{
    public class VanityAddressService(IServiceFactory serviceFactory)
    {
        private int _attemptCount;

        public async Task<Key> Search(
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

            // The timer must be created on the UI thread (the caller) so its dispatcher has a
            // message loop. Creating it inside Task.Run would put it on a thread-pool thread
            // that never pumps messages.
            var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            dispatcherTimer.Tick += (_, _) => SendAttemptCount(attemptCountMessageTokenGuid);
            dispatcherTimer.Start();

            // Send the initial (zero) count to reset the UI.
            SendAttemptCount(attemptCountMessageTokenGuid);

            using var matchCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var matchCt = matchCts.Token;

            try
            {
                var tasks = new Task<Key>[cores];

                for (int i = 0; i < cores; i++)
                {
                    tasks[i] = Task.Run(
                        () => SearchWorker(searchMode, vanityText, minWordLength, isCaseSensitive, isStartsWith, isEndsWith, network, matchCt),
                        matchCt);
                }

                var winningTask = await Task.WhenAny(tasks);

                // A match (or fault) on one task; stop the rest before surfacing the result.
                await matchCts.CancelAsync();

                return await winningTask;
            }
            finally
            {
                // Always stop the timer and flush the final count, even on cancellation or error,
                // otherwise the timer keeps ticking on the UI dispatcher forever.
                dispatcherTimer.Stop();
                SendAttemptCount(attemptCountMessageTokenGuid);
            }
        }

        private void SendAttemptCount(string token)
        {
            WeakReferenceMessenger.Default.Send(_attemptCount.ToString(), token);
        }

        private Key SearchWorker(
            SearchMode searchMode,
            string vanityText,
            int minWordLength,
            bool isCaseSensitive,
            bool isStartsWith,
            bool isEndsWith,
            Network network,
            CancellationToken ct)
        {
            if (searchMode == SearchMode.String)
            {
                var verifier = serviceFactory.GetInputStringVerifierService(vanityText, isCaseSensitive, isStartsWith, isEndsWith);

                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    var privateKey = new Key();
                    var address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network).ToString();
                    Interlocked.Increment(ref _attemptCount);

                    if (verifier.IsVanityAddress(address))
                    {
                        return privateKey;
                    }
                }
            }

            var words = GetWordsHashSet(minWordLength);
            var dictionaryVerifier = serviceFactory.GetDictionaryWordVerifierService(words, isCaseSensitive, isStartsWith, isEndsWith);

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var privateKey = new Key();
                var address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network).ToString();
                Interlocked.Increment(ref _attemptCount);

                if (dictionaryVerifier.IsDictionaryWordAddress(address))
                {
                    return privateKey;
                }
            }
        }

        private static HashSet<string> GetWordsHashSet(int minWordLength)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string dictionaryTxt = "BitcoinVanityAddressFinder.Services.Dictionary.txt";

            using var stream = assembly.GetManifestResourceStream(dictionaryTxt)
                               ?? throw new InvalidOperationException($"Embedded dictionary resource '{dictionaryTxt}' was not found.");
            using var reader = new StreamReader(stream);

            var words = reader.ReadToEnd().Split(["\r\n", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries);

            return words
                .Where(o => o.Length >= minWordLength)
                .ToHashSet();
        }
    }
}
