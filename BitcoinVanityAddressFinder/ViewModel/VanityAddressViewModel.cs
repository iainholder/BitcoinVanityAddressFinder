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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using NBitcoin;

namespace BitcoinVanityAddressFinder.ViewModel
{
    public enum SearchMode
    {
        String,
        Dictionary
    }

    public sealed class VanityAddressViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IServiceFactory _serviceFactory;

        private string _address;
        private int _attemptCount;

        private string _attemptCountMessageTokenGuid = "";

        private CancellationTokenSource _cancellationTokenSource;
        private int _coreComboBoxSelectedItem;
        private int _dictionaryLengthComboBoxSelectedItem;
        private bool _isBeep;
        private bool _isCaseSensitive;
        private bool _isEndsWith;
        private bool _isSearching;
        private bool _isStartsWith;
        private SearchMode _modeComboBoxSelectedItem;
        private Network _networkComboBoxSelectedItem;
        private string _privateKey;
        private string _statusText;
        private string _vanityText;

        public VanityAddressViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;

            SearchCommand = new RelayCommand(Search, CanExecuteSearch);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            VanityText = "";
            IsCaseSensitive = true;

            ModeComboBoxItems = new[] { SearchMode.String, SearchMode.Dictionary };
            ModeComboBoxSelectedItem = SearchMode.String;

            DictionaryLengthComboBoxItems = Enumerable.Range(3, 5);
            DictionaryLengthComboBoxSelectedItem = 3;

            NetworkComboBoxItems = new[] { Network.Main, Network.TestNet, Network.RegTest };
            NetworkComboBoxSelectedItem = Network.Main;

            CoreComboBoxItems = Enumerable.Range(1, Environment.ProcessorCount);
            CoreComboBoxSelectedItem = Environment.ProcessorCount - 1;
        }

        [UsedImplicitly]
        public IEnumerable<int> DictionaryLengthComboBoxItems { get; set; }

        [UsedImplicitly]
        public int DictionaryLengthComboBoxSelectedItem
        {
            get => _dictionaryLengthComboBoxSelectedItem;
            set => Set(ref _dictionaryLengthComboBoxSelectedItem, value);
        }

        [UsedImplicitly]
        public IEnumerable<SearchMode> ModeComboBoxItems { get; set; }

        [UsedImplicitly]
        public SearchMode ModeComboBoxSelectedItem
        {
            get => _modeComboBoxSelectedItem;
            set
            {
                Set(ref _modeComboBoxSelectedItem, value);
                RaisePropertyChanged(nameof(IsStringSearchMode));
            }
        }

        [UsedImplicitly]
        public bool IsStringSearchMode => ModeComboBoxSelectedItem == SearchMode.String;

        [UsedImplicitly]
        public IEnumerable<Network> NetworkComboBoxItems { get; set; }

        [UsedImplicitly]
        public Network NetworkComboBoxSelectedItem
        {
            get => _networkComboBoxSelectedItem;
            set => Set(ref _networkComboBoxSelectedItem, value);
        }

        [UsedImplicitly]
        public IEnumerable<int> CoreComboBoxItems { get; set; }

        [UsedImplicitly]
        public int CoreComboBoxSelectedItem
        {
            get => _coreComboBoxSelectedItem;
            set
            {
                if (value > Environment.ProcessorCount - 1)
                {
                    if (MessageBox.Show($"You should leave one core for Windows and other running processes.{Environment.NewLine}If you are sure you want to use all your CPU cores and understand the implications, press OK. Otherwise press Cancel to default to {Environment.ProcessorCount - 1}.", "Cores", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        // HACK - God I hate this. Find out issue and remove
                        Task.Run(() =>
                        {
                            Thread.Sleep(100);
                            CoreComboBoxSelectedItem = Environment.ProcessorCount - 1;
                        });
                    }
                    else
                    {
                        _coreComboBoxSelectedItem = value;
                    }
                }
                else
                {
                    _coreComboBoxSelectedItem = value;
                }

                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public RelayCommand SearchCommand { get; set; }

        [UsedImplicitly]
        public RelayCommand CancelCommand { get; set; }

        [UsedImplicitly]
        public string VanityText
        {
            get => _vanityText;
            set => Set(ref _vanityText, value.Replace(" ", ""));
        }

        [UsedImplicitly]
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }

        [UsedImplicitly]
        public bool IsSearching
        {
            get => _isSearching;
            set => Set(ref _isSearching, value);
        }

        [UsedImplicitly]
        public string PrivateKey
        {
            get => _privateKey;
            set => Set(ref _privateKey, value);
        }

        [UsedImplicitly]
        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set => Set(ref _isCaseSensitive, value);
        }

        [UsedImplicitly]
        public bool IsStartsWith
        {
            get => _isStartsWith;
            set => Set(ref _isStartsWith, value);
        }

        [UsedImplicitly]
        public bool IsEndsWith
        {
            get => _isEndsWith;
            set => Set(ref _isEndsWith, value);
        }

        [UsedImplicitly]
        public bool IsBeep
        {
            get => _isBeep;
            set => Set(ref _isBeep, value);
        }

        [UsedImplicitly]
        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        [UsedImplicitly]
        public int AttemptCount
        {
            get => _attemptCount;
            set => Set(ref _attemptCount, value);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(VanityText):
                        {
                            if (!VanityText.All(char.IsLetterOrDigit))
                            {
                                Error = "Letters and numbers only";
                                return Error;
                            }

                            if (VanityText.Length >= 8)
                            {
                                Error = "That would take too long";
                                return Error;
                            }

                            if (VanityText.Length > 0 && VanityText.Length < 8)
                            {
                                return "";
                            }

                            return "Error";
                        }
                }

                return "";
            }
        }

        public string Error { get; private set; }

        private bool CanExecuteSearch()
        {
            if (IsStringSearchMode)
            {
                return this[nameof(VanityText)] == "" && !_isSearching;
            }

            return !_isSearching;
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

            Messenger.Default.Register<int>(this, _attemptCountMessageTokenGuid, o =>
            {
                    AttemptCount = o;
                    StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Searching using {CoreComboBoxSelectedItem} core{s} at {AttemptCount / stopwatch.Elapsed.TotalSeconds:N0} keys per second...";
            });

            _cancellationTokenSource = new CancellationTokenSource();
            var ct = _cancellationTokenSource.Token;

            using (var vanityAddressService = _serviceFactory.GetVanityAddressService())
            {
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
                    StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Completed after searching {AttemptCount} keys at {AttemptCount / stopwatch.Elapsed.TotalSeconds:N0} keys per second.";

                    if (IsBeep)
                    {
                        Console.Beep(808, 303);
                    }
                }
                catch (AggregateException ae)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Search cancelled";
                    }
                    else
                    {
                        MessageBox.Show(ae.Flatten().ToString());
                        StatusText = $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] Error";
                    }
                }
                finally
                {
                    stopwatch.Stop();
                    Messenger.Default.Unregister<int>(this, _attemptCountMessageTokenGuid);
                    IsSearching = false;
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }
    }
}