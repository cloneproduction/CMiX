using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class At : ObservableObject
    {
        public At()
        {
            Name = "At";
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private int _hours;
        public int Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }
    }
}
