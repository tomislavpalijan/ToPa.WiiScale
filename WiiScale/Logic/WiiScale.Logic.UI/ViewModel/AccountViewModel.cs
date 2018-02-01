using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Practices.ServiceLocation;
using WiiScale.Logic.UI.BaseClasses;
using WiiScale.Logic.UI.Helper.Navigation;
using WiiScale.Logic.UI.Model;
using WiiScale.Logic.UI.Services.WiiBoard;

namespace WiiScale.Logic.UI.ViewModel
{
    public sealed class AccountViewModel : BaseViewModel
    {
        private const float FloatComparisonTolerance = 0.15f;
        private IWiiBoardService _wiiBoardService;
        
        private float _batteryState;
        
        private float _currentWeight;

        private WiiBoardServiceState _wiiBoardState;


        public AccountViewModel()
        {
            GoBackCommand = new RelayCommand(GoBackExecute);

            ObserveProperty(nameof(CurrentWeight), 800);
            ObserveProperty(nameof(BatteryState), 800);

            if (IsInDesignMode)
            {

            }
            else
            {
                InitializeWiiBoardService();
            }
            
            WeightChartValues = new ChartValues<ObservableValue>();
            WeightLineSeries = new LineSeries {Values = WeightChartValues};
            WeightSeriesCollection = new SeriesCollection {WeightLineSeries};

            Init();
        }

        private void InitializeWiiBoardService()
        {

            _wiiBoardService = ServiceLocator.Current.GetInstance<IWiiBoardService>();

            if (_wiiBoardService == null)
                throw new NullReferenceException();

            _wiiBoardService.WeightInKgChanged += OnWeightInKgChanged;
            _wiiBoardService.BatteryStateChanged += OnBatteryStateChanged;
            _wiiBoardService.WiiBoardServiceStateChanged += OnWiiBoardStateChanged;
        }

        public Account CurrentAccount { get; set; }
        public SlideNavigator SlideNavigator { get; set; }
        public ICommand GoBackCommand { get; }
        public SeriesCollection WeightSeriesCollection { get; set; }
        public ChartValues<ObservableValue> WeightChartValues { get; set; }
        public LineSeries WeightLineSeries { get; set; }

        /// <summary>
        ///     Gets the current wii board state
        /// </summary>
        public WiiBoardServiceState WiiBoardState
        {
            get => _wiiBoardState;
            private set
            {
                if (_wiiBoardState == value)
                    return;

                _wiiBoardState = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the current battery state
        /// </summary>
        public float BatteryState
        {
            get => _batteryState;
            private set
            {
                if (Math.Abs(_batteryState - value) < FloatComparisonTolerance)
                    return;

                _batteryState = value;

                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the current messured weight
        /// </summary>
        public float CurrentWeight
        {
            get => _currentWeight;
            private set
            {
                if (Math.Abs(_currentWeight - value) < FloatComparisonTolerance)
                    return;

                _currentWeight = value;

                RaisePropertyChanged();
            }
        }

        private void Init()
        {
            if (_wiiBoardService != null && !_wiiBoardService.IsInitialized)
                _wiiBoardService.Init();
        }

        private void OnWiiBoardStateChanged(object sender, WiiBoardServiceState state)
        {
            var wii = sender as IWiiBoardService;

            if (wii == null)
                return;

            WiiBoardState = state;
        }

        private void OnBatteryStateChanged(object sender, float batteryState)
        {
            var wii = sender as IWiiBoardService;

            if (wii == null)
                return;

            BatteryState = batteryState;
        }

        private void OnWeightInKgChanged(object sender, float weight)
        {
            var wii = sender as IWiiBoardService;

            if (wii == null)
                return;

            CurrentWeight = weight;
        }

        private void GoBackExecute()
        {
            SlideNavigator.GoBack();
        }

        public void Show(Account account)
        {
            CurrentAccount = account;

            WeightChartValues.Clear();

            foreach (var weight in CurrentAccount.WeightsCollection)
                WeightChartValues.Add(new ObservableValue(weight.Value));
        }

        public override void Dispose()
        {
            if (_wiiBoardService != null)
            {
                _wiiBoardService.BatteryStateChanged -= OnBatteryStateChanged;
                _wiiBoardService.WeightInKgChanged -= OnWeightInKgChanged;
                _wiiBoardService.WiiBoardServiceStateChanged -= OnWiiBoardStateChanged;
            }

            base.Dispose();
        }
    }
}