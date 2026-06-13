using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BitcoinVanityAddressFinder.Annotations;
using BitcoinVanityAddressFinder.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NBitcoin;

namespace BitcoinVanityAddressFinder.ViewModel
{
    public enum SearchMode
    {
        String,
        Dictionary
    }

    public sealed class VanityAddressViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly IServiceFactory _serviceFactory;

        private string _attemptCountMessageTokenGuid = "";

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isSearching;

        public VanityAddressViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;

            SearchCommand = new RelayCommand(Search, CanExecuteSearch);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            CopyAddressCommand = new RelayCommand(() => CopyToClipboard(Address), () => !string.IsNullOrEmpty(Address));
            CopyPrivateKeyCommand = new RelayCommand(() => CopyToClipboard(PrivateKey), () => !string.IsNullOrEmpty(PrivateKey));

            VanityText = "";
            IsCaseSensitive = true;

            ModeComboBoxItems = [SearchMode.String, SearchMode.Dictionary];
            ModeComboBoxSelectedItem = SearchMode.String;

            DictionaryLengthComboBoxItems = Enumerable.Range(3, 5);
            DictionaryLengthComboBoxSelectedItem = 3;

            NetworkComboBoxItems = [Network.Main, Network.TestNet, Network.RegTest];
            NetworkComboBoxSelectedItem = Network.Main;

            CoreComboBoxItems = Enumerable.Range(1, Environment.ProcessorCount);
            CoreComboBoxSelectedItem = Environment.ProcessorCount - 1;
        }

        [UsedImplicitly]
        public IEnumerable<int> DictionaryLengthComboBoxItems { get; set; }

        [UsedImplicitly]
        public int DictionaryLengthComboBoxSelectedItem
        {
            get;
            set => SetProperty(ref field, value);
        }

        [UsedImplicitly]
        public IEnumerable<SearchMode> ModeComboBoxItems { get; set; }

        [UsedImplicitly]
        public SearchMode ModeComboBoxSelectedItem
        {
            get;
            set
            {
                SetProperty(ref field, value);
                OnPropertyChanged(nameof(IsStringSearchMode));
                SearchCommand.NotifyCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public bool IsStringSearchMode => ModeComboBoxSelectedItem == SearchMode.String;

        [UsedImplicitly]
        public IEnumerable<Network> NetworkComboBoxItems { get; set; }

        [UsedImplicitly]
        public Network NetworkComboBoxSelectedItem
        {
            get;
            set => SetProperty(ref field, value);
        }

        [UsedImplicitly]
        public IEnumerable<int> CoreComboBoxItems { get; set; }

        [UsedImplicitly]
        public int CoreComboBoxSelectedItem
        {
            get;
            set
            {
                if (value > Environment.ProcessorCount - 1)
                {
                    if (MessageBox.Show($"You should leave one core for Windows and other running processes.{Environment.NewLine}" +
                                        $"If you are sure you want to use all your CPU cores and understand the implications, press OK. Otherwise press Cancel to default to {Environment.ProcessorCount - 1}.", "Cores",
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        field = Environment.ProcessorCount - 1;
                    }
                    else
                    {
                        field = value;
                    }
                }
                else
                {
                    field = value;
                }

                OnPropertyChanged();
            }
        }

        [UsedImplicitly]
        public RelayCommand SearchCommand { get; set; }

        [UsedImplicitly]
        public RelayCommand CancelCommand { get; set; }

        [UsedImplicitly]
        public RelayCommand CopyAddressCommand { get; }

        [UsedImplicitly]
        public RelayCommand CopyPrivateKeyCommand { get; }

        [UsedImplicitly]
        public string VanityText
        {
            get;
            set
            {
                SetProperty(ref field, value.Replace(" ", ""));
                SearchCommand.NotifyCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public string Address
        {
            get;
            set
            {
                SetProperty(ref field, value);
                CopyAddressCommand.NotifyCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                SetProperty(ref _isSearching, value);
                SearchCommand.NotifyCanExecuteChanged();
                CancelCommand.NotifyCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public string PrivateKey
        {
            get;
            set
            {
                SetProperty(ref field, value);
                CopyPrivateKeyCommand.NotifyCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public bool IsCaseSensitive
        {
            get;
            set
            {
                SetProperty(ref field, value);
                // Which characters are impossible in an address depends on case sensitivity,
                // so re-run validation of the vanity text whenever this toggles.
                OnPropertyChanged(nameof(VanityText));
                SearchCommand.NotifyCanExecuteChanged();
            }
        }

        [UsedImplicitly]
        public bool IsStartsWith
        {
            get;
            set => SetProperty(ref field, value);
        }

        [UsedImplicitly]
        public bool IsEndsWith
        {
            get;
            set => SetProperty(ref field, value);
        }

        [UsedImplicitly]
        public bool IsBeep
        {
            get;
            set => SetProperty(ref field, value);
        }

        [UsedImplicitly]
        public string StatusText
        {
            get;
            set => SetProperty(ref field, value);
        }

        [UsedImplicitly]
        public int AttemptCount
        {
            get;
            set => SetProperty(ref field, value);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(VanityText):
                        {
                            // Empty is not an error to display, but Search stays disabled (see CanExecuteSearch).
                            if (VanityText.Length == 0)
                            {
                                return "";
                            }

                            if (!VanityText.All(char.IsLetterOrDigit))
                            {
                                return Error = "Letters and numbers only";
                            }

                            if (VanityText.Length >= 8)
                            {
                                return Error = "That would take too long";
                            }

                            string impossible = GetImpossibleBase58Characters(VanityText);
                            if (impossible.Length > 0)
                            {
                                return Error = $"A Bitcoin address can never contain: {string.Join(' ', impossible.ToCharArray())}";
                            }

                            return "";
                        }
                }

                return "";
            }
        }

        public string Error { get; private set; }

        // The Base58 alphabet Bitcoin uses deliberately omits 0 (zero), O, I and l to avoid
        // visual ambiguity. Vanity text containing only-impossible characters could never match,
        // so we surface it instead of launching an unwinnable, never-ending search.
        private const string Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        private string GetImpossibleBase58Characters(string text)
        {
            var impossible = text
                .Where(c => !Base58Alphabet.Contains(c)
                            && (IsCaseSensitive || !Base58Alphabet.Contains(SwapCase(c))))
                .Distinct()
                .ToArray();

            return new string(impossible);
        }

        private static char SwapCase(char c) =>
            char.IsUpper(c) ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c);

        private static string FormatRate(int attempts, TimeSpan elapsed) =>
            elapsed.TotalSeconds > 0 ? $"{attempts / elapsed.TotalSeconds:N0}" : "0";

        private static void CopyToClipboard(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                Clipboard.SetText(text);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                // The clipboard is occasionally locked by another process; ignore rather than crash.
            }
        }

        private bool CanExecuteSearch()
        {
            if (_isSearching)
            {
                return false;
            }

            if (IsStringSearchMode)
            {
                return VanityText.Length > 0 && this[nameof(VanityText)] == "";
            }

            return true;
        }

        private void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        private bool CanCancel()
        {
            return IsSearching;
        }

        private async void Search()
        {
            Address = "";
            PrivateKey = "";
            IsSearching = true;
            string s = CoreComboBoxSelectedItem == 1 ? "" : "s";
            StatusText = $"[00:00:00] Searching using {CoreComboBoxSelectedItem} core{s}...";
            AttemptCount = 0;
            _attemptCountMessageTokenGuid = Guid.NewGuid().ToString();

            var stopwatch = new Stopwatch();

            WeakReferenceMessenger.Default.Register<string, string>(this, _attemptCountMessageTokenGuid, (recipient, message) =>
            {
                var vm = (VanityAddressViewModel)recipient;
                vm.AttemptCount = int.Parse(message);
                vm.StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Searching using {vm.CoreComboBoxSelectedItem} core{s} at {FormatRate(vm.AttemptCount, stopwatch.Elapsed)} keys per second...";
            });

            _cancellationTokenSource = new CancellationTokenSource();
            var ct = _cancellationTokenSource.Token;

            var vanityAddressService = _serviceFactory.GetVanityAddressService();

            try
            {
                stopwatch.Start();

                var result = await vanityAddressService.Search(
                    CoreComboBoxSelectedItem,
                    ModeComboBoxSelectedItem,
                    VanityText,
                    DictionaryLengthComboBoxSelectedItem,
                    IsCaseSensitive,
                    IsStartsWith,
                    IsEndsWith,
                    NetworkComboBoxSelectedItem,
                    _attemptCountMessageTokenGuid,
                    ct);

                stopwatch.Stop();

                var vanityPrivateKey = result;
                Address = vanityPrivateKey?.PubKey.GetAddress(ScriptPubKeyType.Legacy, NetworkComboBoxSelectedItem).ToString();
                PrivateKey = vanityPrivateKey?.GetWif(NetworkComboBoxSelectedItem).ToString();
                StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Completed after searching {AttemptCount:N0} keys at {FormatRate(AttemptCount, stopwatch.Elapsed)} keys per second.";

                if (IsBeep)
                {
                    Console.Beep(808, 303);
                }
            }
            catch (OperationCanceledException)
            {
                StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Search cancelled";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Error";
            }
            finally
            {
                stopwatch.Stop();
                WeakReferenceMessenger.Default.Unregister<string, string>(this, _attemptCountMessageTokenGuid);
                IsSearching = false;
                await _cancellationTokenSource.CancelAsync();
                _cancellationTokenSource.Dispose();
                SearchCommand.NotifyCanExecuteChanged();
            }
        }
    }
}