using System;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using WiiScale.Logic.UI.BaseClasses;
using WiiScale.Logic.UI.Helper.Navigation;
using WiiScale.Logic.UI.Model;

namespace WiiScale.Logic.UI.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        public Account CurrentAccount { get; set; }
        public SlideNavigator SlideNavigator { get; set; }
        public ICommand GoBackCommand { get; private set; }
        public SeriesCollection WeightSeriesCollection { get; set; }
        public ChartValues<ObservableValue> WeightChartValues { get; set; }
        public LineSeries WeightLineSeries { get; set; }


        public AccountViewModel()
        {
            GoBackCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(GoBackExecute);

            WeightChartValues = new ChartValues<ObservableValue>();
            WeightLineSeries = new LineSeries {Values = WeightChartValues};
            WeightSeriesCollection = new SeriesCollection {WeightLineSeries};
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
            {
                WeightChartValues.Add(new ObservableValue(weight.Value));
            }
        }
    }
}