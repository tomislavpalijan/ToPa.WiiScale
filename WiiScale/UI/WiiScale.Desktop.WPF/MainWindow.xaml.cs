using System.ComponentModel;
using System.Windows;
using WiiScale.Logic.UI.ViewModel;

namespace WiiScale.Desktop.WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            // ToDo: dirty approach. Have to find a better solution.
            var vm = DataContext as MainViewModel;
            vm?.Dispose();
        }

        private void ExitButtonClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
