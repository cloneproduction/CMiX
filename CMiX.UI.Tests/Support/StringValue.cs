using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.UI.Tests.Support
{
    // A model for the editors that bind Value and ResetCommand, such as the ColorSelector.
    public class StringValue : ObservableObject
    {
        private string _value = "#FF000000";

        public StringValue()
        {
            ResetCommand = new RelayCommand(() => Value = "#FF000000");
        }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public ICommand ResetCommand { get; }
    }
}
