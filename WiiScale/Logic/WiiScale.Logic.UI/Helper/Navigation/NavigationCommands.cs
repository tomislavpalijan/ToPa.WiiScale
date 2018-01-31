using System.Windows.Input;

namespace WiiScale.Logic.UI.Helper.Navigation
{
    public static class NavigationCommands
    {
        public static RoutedCommand ShowAccountCommand = new RoutedCommand();
        public static RoutedCommand ShowWeightCommand = new RoutedCommand();
        public static RoutedCommand GoBackCommand = new RoutedCommand();
    }
}