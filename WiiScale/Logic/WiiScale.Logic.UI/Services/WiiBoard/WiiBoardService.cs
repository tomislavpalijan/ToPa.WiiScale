using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WiimoteLib;
using Timer = System.Timers.Timer;

namespace WiiScale.Logic.UI.Services.WiiBoard
{
    public enum WiiBoardServiceState
    {
        Unknown,
        Discover,
        Connect,
        Calibration,
        Ready
    }

    public class WiiBoardService : IWiiBoardService
    {
        private readonly object _lockObj = new object();
        private readonly List<float> _offsetRecord;
        private Timer _aliveTimer;

        private float _batteryState;
        private bool _foundDebice;
        private bool _isFirststart = true;
        private float _weightInKg;
        private float _weightInKgRaw;
        private Wiimote _wiiBalanceBoard;
        private WiiBoardServiceState _wiiBoardServiceState;
        public event EventHandler<float> BatteryStateChanged;
        public event EventHandler<float> OffsetChanged;
        public event EventHandler<float> WeightInKgChanged;
        public event EventHandler<WiiBoardServiceState> WiiBoardServiceStateChanged;

        public WiiBoardService()
        {
            WiiBoardServiceState = WiiBoardServiceState.Unknown;
            _offsetRecord = new List<float>();
            _foundDebice = false;
            IsInitialized = false;
            WiimoteCollection = new WiimoteCollection();
        }

        public float BatteryState
        {
            get => _batteryState;
            private set
            {
                _batteryState = value;
                BatteryStateChanged?.Invoke(this, _batteryState);
            }
        }

        public WiimoteCollection WiimoteCollection { get; }

        public WiiBoardServiceState WiiBoardServiceState
        {
            get => _wiiBoardServiceState;
            private set
            {
                _wiiBoardServiceState = value;
                WiiBoardServiceStateChanged?.Invoke(this, _wiiBoardServiceState);
            }
        }


        public bool IsInitialized { get; private set; }
        public float Offset { get; private set; }
        public float OffsetMin { get; private set; }
        public float OffsetMax { get; private set; }
        public float OffsetRange { get; private set; }


        public float WeightInKg
        {
            get => _weightInKg;
            private set
            {
                _weightInKg = value;
                WeightInKgChanged?.Invoke(this, _weightInKg);
            }
        }

        public void Init(float offset = 0.0f)
        {
            Offset = offset;
            _aliveTimer = new Timer(3000)
            {
                AutoReset = true
            };

            _aliveTimer.Elapsed += AliveTimerOnElapsed;
            IsInitialized = true;
            WiiBoardServiceState = WiiBoardServiceState.Discover;
            _aliveTimer.Start();
        }

        public void StartCalibration()
        {
            lock (_lockObj)
            {
                _isFirststart = false;
            }

            WiiBoardServiceState = WiiBoardServiceState.Calibration;

            Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < 30; i++)
                {
                    OnRecordOffset();
                    Thread.Sleep(1000);
                }

                CalculateOffset();
            });
        }

        private void AliveTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_foundDebice)
                return;

            if (_wiiBalanceBoard != null)
            {
                _wiiBalanceBoard.WiimoteChanged -= WiiMoteOnWiimoteChanged;
                _wiiBalanceBoard.Dispose();
            }

            WiimoteCollection.Clear();

            try
            {
                WiimoteCollection.FindAllWiimotes();
            }
            catch (WiimoteException )
            {
                WiiBoardServiceState = WiiBoardServiceState.Discover;
                _foundDebice = false;
                return;
            }
            catch (Exception)
            {
                WiiBoardServiceState = WiiBoardServiceState.Discover;
                _foundDebice = false;
                return;
            }

            foreach (var wiiMote in WiimoteCollection)
            {
                wiiMote.Connect();

                if (wiiMote.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                    continue;

                _wiiBalanceBoard = wiiMote;
                WiiBoardServiceState = WiiBoardServiceState.Connect;
                _wiiBalanceBoard.WiimoteChanged += WiiMoteOnWiimoteChanged;
                _foundDebice = true;
            }

            lock (_lockObj)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (!_isFirststart || Offset != 0.0f)
                {
                    _isFirststart = false;
                    WiiBoardServiceState = WiiBoardServiceState.Ready;
                    return;
                }
            }

            StartCalibration();
        }

        private void CalculateOffset()
        {
            OffsetMax = _offsetRecord.Max();
            OffsetMin = _offsetRecord.Min();
            OffsetRange = OffsetMax - OffsetMin;
            Offset = _offsetRecord.Average();

            if (OffsetRange > 2.0f)
                Offset = 0.0f;

            OffsetChanged?.Invoke(this, Offset);

            WiiBoardServiceState = WiiBoardServiceState.Ready;
        }

        private void OnRecordOffset()
        {
            _offsetRecord.Add(_weightInKgRaw);
        }

        private void WiiMoteOnWiimoteChanged(object sender, WiimoteChangedEventArgs wiimoteChangedEventArgs)
        {
            var wiiMoteState = wiimoteChangedEventArgs.WiimoteState;

            switch (wiiMoteState.ExtensionType)
            {
                case ExtensionType.None:
                    break;
                case ExtensionType.Nunchuk:
                    break;
                case ExtensionType.ClassicController:
                    break;
                case ExtensionType.Guitar:
                    break;
                case ExtensionType.Drums:
                    break;
                case ExtensionType.BalanceBoard:
                    _weightInKgRaw = wiiMoteState.BalanceBoardState.WeightKg;
                    WeightInKg = wiiMoteState.BalanceBoardState.WeightKg + Offset;
                    BatteryState = wiiMoteState.Battery;
                    break;
                case ExtensionType.ParitallyInserted:
                    break;
                default:
                    throw new NotImplementedException("WiiBoardService: Wii mote extension type not implemented");
            }
        }
    }
}