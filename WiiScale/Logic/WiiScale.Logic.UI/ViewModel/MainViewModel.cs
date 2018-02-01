using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using WiiScale.Logic.UI.BaseClasses;
using WiiScale.Logic.UI.Helper.Navigation;
using WiiScale.Logic.UI.Model;
using WiiScale.Logic.UI.Services.WiiBoard;

namespace WiiScale.Logic.UI.ViewModel
{
    public class MainViewModel : BaseViewModel, ISlideNavigationSubject
    {
        public SlideNavigator SlideNavigator { get; private set; }
        public ICommand ShowNewAccountCommand { get; private set; }
        public IWiiBoardService WiiBoardService { get; set; }

        public MainViewModel()
        {


            Init();

            InitCommands();

            if (IsInDesignMode)
            {
                WindowTitle = "WiiScale DesignMode";
                
            }
            else
            {
                WindowTitle = "Wii Scale";
                WiiBoardService = ServiceLocator.Current.GetInstance<IWiiBoardService>();
            }
            
            SlideNavigator = new SlideNavigator(this, Accounts);
            NewAccountViewModel.SlideNavigator = SlideNavigator;
            AccountViewModel.SlideNavigator = SlideNavigator;
            WeightsViewModel.SlideNavigator = SlideNavigator;
            SlideNavigator.GoTo(0);
        }

        private void InitCommands()
        {
            ShowNewAccountCommand = new RelayCommand(ShowNewAccount());


            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Helper.Navigation.NavigationCommands.ShowAccountCommand, ShowCurrentAccount ));
        }

        private void ShowCurrentAccount(object sender, ExecutedRoutedEventArgs e)
        {
            SlideNavigator.GoTo(
                IndexOfSlide<AccountViewModel>(), () => AccountViewModel.Show((Account) e.Parameter));
        }

        private Action ShowNewAccount()
        {
            return () =>
            {
                SlideNavigator.GoTo(
                    IndexOfSlide<NewAccountViewModel>(),
                    () => NewAccountViewModel.Show(AccountSet));
            };
        }


        private void Init()
        {
            Accounts = new ObservableCollection<object>
            {
                AccountSet,
                AccountViewModel,
                WeightsViewModel,
                NewAccountViewModel
            };

        }

        private int IndexOfSlide<TSlide>()
        {
            return Accounts.Select((o, i) => new { o, i }).First(a => a.o.GetType() == typeof(TSlide)).i;
        }

        public ObservableCollection<object> Accounts { get; private set; }
        public AccountSet AccountSet { get; set; } = new AccountSet();
        public AccountViewModel AccountViewModel { get; set; } = new AccountViewModel();
        public WeightsViewModel WeightsViewModel { get; set; } = new WeightsViewModel();
        public NewAccountViewModel NewAccountViewModel { get; set; } = new NewAccountViewModel();
        public int ActiveSlideIndex { get; set; }

        public override void Dispose()
        {
            base.Dispose();

            AccountSet.Dispose();
        }
    }
}