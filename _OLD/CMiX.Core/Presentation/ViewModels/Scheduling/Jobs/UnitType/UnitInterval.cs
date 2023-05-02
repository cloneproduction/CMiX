using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class UnitInterval : ObservableObject
    {
        public UnitInterval(int interval)
        {
            Interval = interval;
            AddCommand = new RelayCommand(Add);
            SubCommand = new RelayCommand(Sub);
        }

        public ICommand AddCommand { get; set; }
        public ICommand SubCommand { get; set; }

        private int _interval;
        public int Interval
        {
            get => _interval;
            set => SetProperty(ref _interval, value);
        }

        private void Add()
        {
            Interval += 1;
        }

        private void Sub()
        {
            if (Interval > 1)
                Interval -= 1;
        }
    }
}
