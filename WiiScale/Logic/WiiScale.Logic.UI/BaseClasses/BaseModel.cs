using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using WiiScale.Logic.UI.Annotations;
using System.Windows.Threading;

namespace WiiScale.Logic.UI.BaseClasses
{
    public abstract class BaseModel : INotifyPropertyChanged, IDataErrorInfo, IDisposable
    {
        private static List<PropertyInfo> _propertyInfos;

        private readonly Dictionary<string, DispatcherTimer> _disTimers = new Dictionary<string, DispatcherTimer>();

        protected BaseModel()
        {
            InitCommands();
        }

        public bool HasErrors => Errors.Any();

        public bool IsOk => !HasErrors;

        protected List<PropertyInfo> PropertyInfos

        {
            get
            {
                return _propertyInfos
                       ?? (_propertyInfos =
                           GetType()
                               .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(prop => prop.IsDefined(typeof(RequiredAttribute), true) ||
                                              prop.IsDefined(typeof(MaxLengthAttribute), true))
                               .ToList());
            }
        }


        private Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        public string this[string columnName]
        {
            get

            {
                CollectErrors();

                return Errors.ContainsKey(columnName) ? Errors[columnName] : string.Empty;
            }
        }

        public string Error => string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void InitCommands()
        {
        }
        
        protected virtual void OnErrorsCollected()
        {
        }

        private void CollectErrors()
        {
            Errors.Clear();
            PropertyInfos.ForEach(
                prop =>
                {
                    var currentValue = prop.GetValue(this);
                    var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                    var maxLenAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
                    if (requiredAttr != null)
                    {
                        if (string.IsNullOrEmpty(currentValue?.ToString() ?? string.Empty))
                        {
                            Errors.Add(prop.Name, requiredAttr.ErrorMessage);
                        }
                    }
                    if (maxLenAttr != null)
                    {
                        if ((currentValue?.ToString() ?? string.Empty).Length > maxLenAttr.Length)
                        {
                            Errors.Add(prop.Name, maxLenAttr.ErrorMessage);
                        }
                    }
                    // TODO further attributes
                });
            // we have to this because the Dictionary does not implement INotifyPropertyChanged            
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(IsOk));
            // commands do not recognize property changes automatically
            OnErrorsCollected();
        }


        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null && !_disTimers.ContainsKey(propertyName))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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
            var handler = PropertyChanged;

            if (handler != null)
            {
                var propertyName = (from t in _disTimers
                    where t.Value.Equals(sender)
                    select t.Key).FirstOrDefault();

                if (propertyName != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
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
                    OnPropertyChanged(property);
                }
            }
        }
    }
}