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
using NBitcoin;

namespace BitcoinVanityAddressFinder.ViewModel
{
    public sealed class VanityAddressViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _address;

        private CancellationTokenSource _cancellationTokenSource;
        private int _coreComboBoxSelectedItem;
        private bool _isBeep;
        private bool _isCaseSensitive;
        private bool _isEndsWith;
        private bool _isSearching;
        private bool _isStartsWith;
        private Network _networkComboBoxSelectedItem;
        private string _privateKey;
        private string _statusText;
        private string _vanityText;

        public VanityAddressViewModel()
        {
            SearchCommand = new RelayCommand(Search, CanExecuteSearch);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            VanityText = "";
            IsCaseSensitive = true;

            NetworkComboBoxItems = new[] { Network.Main, Network.TestNet, Network.RegTest };
            NetworkComboBoxSelectedItem = Network.Main;

            CoreComboBoxItems = Enumerable.Range(1, Environment.ProcessorCount);
            CoreComboBoxSelectedItem = Environment.ProcessorCount - 1;
        }

        [UsedImplicitly]
        public IEnumerable<Network> NetworkComboBoxItems { get; set; }

        [UsedImplicitly]
        public Network NetworkComboBoxSelectedItem
        {
            get { return _networkComboBoxSelectedItem; }
            set
            {
                _networkComboBoxSelectedItem = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public IEnumerable<int> CoreComboBoxItems { get; set; }

        [UsedImplicitly]
        public int CoreComboBoxSelectedItem
        {
            get { return _coreComboBoxSelectedItem; }
            set
            {
                if (value > Environment.ProcessorCount - 1)
                {
                    if (MessageBox.Show($"You should leave one core for Windows and other running processes.{Environment.NewLine}If you are sure you want to use all your CPU cores and understand the implications, press OK. Otherwise press Cancel to default to {Environment.ProcessorCount - 1}.", "Cores", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        // HACK
                        Task.Factory.StartNew(() =>
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
            get { return _vanityText; }
            set
            {
                _vanityText = value.Replace(" ", "");
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public string PrivateKey
        {
            get { return _privateKey; }
            set
            {
                _privateKey = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public bool IsCaseSensitive
        {
            get { return _isCaseSensitive; }
            set
            {
                _isCaseSensitive = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public bool IsStartsWith
        {
            get { return _isStartsWith; }
            set
            {
                _isStartsWith = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public bool IsEndsWith
        {
            get { return _isEndsWith; }
            set
            {
                _isEndsWith = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public bool IsBeep
        {
            get { return _isBeep; }
            set
            {
                _isBeep = value;
                RaisePropertyChanged();
            }
        }

        [UsedImplicitly]
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                RaisePropertyChanged();
            }
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
            return this[nameof(VanityText)] == "" && !_isSearching;
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Address = "";
            PrivateKey = "";
            StatusText = $"Using {CoreComboBoxSelectedItem} cores";
            IsSearching = true;

            _cancellationTokenSource = new CancellationTokenSource();
            var ct = _cancellationTokenSource.Token;

            try
            {
                var result = await new VanityAddressService().Search(
                    CoreComboBoxSelectedItem,
                    VanityText,
                    IsCaseSensitive,
                    IsStartsWith,
                    IsEndsWith,
                    NetworkComboBoxSelectedItem,
                    ct);

                var vanityPrivateKey = result.PrivateKey;
                Address = vanityPrivateKey?.PubKey.GetAddress(NetworkComboBoxSelectedItem).ToString();
                PrivateKey = vanityPrivateKey?.GetWif(NetworkComboBoxSelectedItem).ToString();
                StatusText = $"{result.AttemptCount} attempts in {stopwatch.Elapsed:g} ({(float)result.AttemptCount / stopwatch.ElapsedMilliseconds:N3}/ms)";

                if (IsBeep)
                {
                    Console.Beep(808, 303);
                }
            }
            catch (AggregateException ae)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    StatusText = "Cancelled";
                }
                else
                {
                    // TODO - Flatten and display inner exceptions
                    MessageBox.Show(ae.Message);
                    StatusText = "Error";
                }
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                IsSearching = false;
                SearchCommand.RaiseCanExecuteChanged();
            }
        }
    }
}