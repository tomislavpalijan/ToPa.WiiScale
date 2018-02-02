using System;
using System.Collections.ObjectModel;
using System.Linq;
using WiiScale.Logic.UI.BaseClasses;

namespace WiiScale.Logic.UI.Model
{
    public class Account : BaseViewModel , ICloneable
    {
        public Person Person { get; set; } = new Person();
        public ObservableCollection<Weight> WeightsCollection { get; private set; }
        
        private float? _currentWeight;

        public float? CurrentWeight
        {
            get { return _currentWeight; }
            private set
            {
                _currentWeight = value;
                RaisePropertyChanged();
            }
        }

        private double? _bodyMassIndex;

        public double? BodyMassIndex
        {
            get { return _bodyMassIndex; }
            private set
            {
                _bodyMassIndex = value;
                RaisePropertyChanged();
            }
        }
        
        public Account()
        {
            Init();
        }

        private void Init()
        {
            WeightsCollection = new ObservableCollection<Weight>();
            WeightsCollection.CollectionChanged += (sender, args) =>
            {
                CurrentWeight = WeightsCollection.Count > 0
                    ? WeightsCollection.LastOrDefault().Value
                    : default(float);

                BodyMassIndex = (Person.Height.HasValue && CurrentWeight.HasValue)
                    ?  (double) CurrentWeight / ((double) Person.Height * (double) Person.Height / 10000 ): default(double?);
            };

        }

        public Account(Person person)
        {
            Init();
            Person = person;
        }

        public Account(Person person, Weight weight)
        {
            Init();
            Person = person;
            WeightsCollection.Add(weight);

        }

        public object Clone()
        {
            var person = (Person) Person.Clone();

            return new Account(person, new Weight());
        }
    }
}