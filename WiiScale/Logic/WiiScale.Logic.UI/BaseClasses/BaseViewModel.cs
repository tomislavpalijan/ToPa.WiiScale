using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace WiiScale.Logic.UI.BaseClasses
{
    public abstract class BaseViewModel : ViewModelBase
    {
        private readonly Dictionary<string, DispatcherTimer> _disTimers = new Dictionary<string, DispatcherTimer>();

        protected BaseViewModel()
        {
            if (!IsInDesignModeStatic && !IsInDesignMode)
            {
                DispatcherHelper.Initialize();
            }
        }

        /// <summary>
        /// A property that indicates if the view model state is valid.
        /// </summary>
        public bool ValidationOk { get; set; } = true;

        /// <summary>
        /// The caption of the window.
        /// </summary>
        public string WindowTitle { get; protected set; }

        public override void RaisePropertyChanged(string propertyName = null)
        {
            if(string.IsNullOrEmpty(propertyName))
                return;

            if (!_disTimers.ContainsKey(propertyName))
            {
                base.RaisePropertyChanged(propertyName);
            }
        }


        public virtual void ObserveProperty(string propertyName)
        {
            ObserveProperty(propertyName, 500);
        }


        public virtual void ObserveProperty(string propertyname, int milliseconds)
        {
            milliseconds = milliseconds < 500 ? 500 : milliseconds;
            DispatcherTimer timer;
            if (_disTimers.ContainsKey(propertyname))
            {
                timer = _disTimers[propertyname];
            }
            else
            {
                timer = new DispatcherTimer();
                _disTimers.Add(propertyname, timer);
            }
            timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
            timer.Tick += TimerOnTick;
            timer.Start();
        }



        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var propertyName = (from t in _disTimers
                where t.Value.Equals(sender)
                select t.Key).FirstOrDefault();

            if (propertyName != null)
            {
                base.RaisePropertyChanged(propertyName);
            }
        }

        public virtual void EndObserveProperty(string propertyName)
        {
            if (_disTimers.ContainsKey(propertyName))
            {
                _disTimers[propertyName].Start();
                _disTimers[propertyName].Tick -= TimerOnTick;
                _disTimers.Remove(propertyName);
            }
        }

        public virtual void EndAllObserveProperties()
        {
            foreach (var timer in _disTimers.Values)
            {
                timer.Stop();
                timer.Tick -= TimerOnTick;
            }
            _disTimers.Clear();
        }

        public virtual void Dispose()
        {
            EndAllObserveProperties();
        }


        protected void ReevaluateProperties(string s, List<string> _propertyList)
        {
            foreach (var property in _propertyList)
            {
                if (!string.Equals(property, s))
                {
                    base.RaisePropertyChanged(property);
                }
            }
        }
    }
}