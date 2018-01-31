using System;
using System.Collections.ObjectModel;
using System.Linq;
using WiiScale.Logic.UI.BaseClasses;

namespace WiiScale.Logic.UI.Model
{
    public class Account : BaseModel, ICloneable
    {
        public Person Person { get; set; } = new Person();
        public ObservableCollection<Weight> WeightsCollection { get; private set; }

        public float? CurrentWeight => WeightsCollection.Count > 0
            ? WeightsCollection.LastOrDefault().Value
            : default(float);

        public double? BodyMassIndex => (Person.Height.HasValue && CurrentWeight.HasValue)
            ?  (double) CurrentWeight / ((double) Person.Height * (double) Person.Height / 10000 ): default(double?);
        
        public Account()
        {
            Init();
        }

        private void Init()
        {
            WeightsCollection = new ObservableCollection<Weight>();

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