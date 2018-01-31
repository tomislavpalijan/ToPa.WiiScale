using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using WiiScale.Logic.UI.BaseClasses;
using WiiScale.Logic.UI.Helper.Navigation;
using WiiScale.Logic.UI.Model;

namespace WiiScale.Logic.UI.ViewModel
{
    public class NewAccountViewModel : BaseViewModel
    {
        private AccountSet _accountSet;

        public NewAccountViewModel()
        {
            AddAccountCommand = new RelayCommand(() =>
            {
                _accountSet.Accounts.Add((Account) Account.Clone());
            });

            GoBackCommand = new RelayCommand(() =>
            {
                if (SlideNavigator == null) return;
                SlideNavigator.GoBack();
            });
        }

        public Account Account { get; set; } = new Account();
        public SlideNavigator SlideNavigator { get; set; }

        public ICommand AddAccountCommand { get; set; }
        public ICommand GoBackCommand { get; set; }

        public void Show(AccountSet accountSet)
        {
            _accountSet = accountSet ?? throw new ArgumentNullException(nameof(accountSet));
            
        }
    }
}