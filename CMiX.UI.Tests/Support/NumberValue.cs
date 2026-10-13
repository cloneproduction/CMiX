using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.UI.Tests.Support
{
    // A model for the number editors. Value and ResetCommand are the names the editors bind.
    public class NumberValue : ObservableObject
    {
        private double _value;

        public NumberValue(double value = 0)
        {
            _value = value;
            ResetCommand = new RelayCommand(() => Value = 0);
        }

        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public ICommand ResetCommand { get; }
    }
}
