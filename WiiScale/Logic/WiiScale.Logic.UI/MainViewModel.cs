using GalaSoft.MvvmLight;

namespace WiiScale.Logic.UI
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                Title = "WiiScale DesignMode";
            }
            else
            {
                Title = "WiiScale";
            }
        }

        public string Title { get; set; }
    }
}