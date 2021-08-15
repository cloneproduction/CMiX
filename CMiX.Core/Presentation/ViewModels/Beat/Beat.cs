// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public abstract class Beat : ObservableRecipient
    {
        public Beat(BeatModel beatModel)
        {
            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand<MasterBeat>(Multiply);
            DivideCommand = new RelayCommand<MasterBeat>(Divide);
        }


        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public abstract double Period { get; set; }


        private double _bpm;
        public double BPM
        {
            get
            {
                _bpm = 60000 / Period;
                if (double.IsInfinity(_bpm) || double.IsNaN(_bpm))
                    return 0;
                else
                    return _bpm;
            }
            set
            {
                Period = 60000 / value;
                SetProperty(ref _bpm, value);
            }
        }

        private double _multiplier;
        public virtual double Multiplier
        {
            get => _multiplier;
            set => SetProperty(ref _multiplier, value);
        }

        private void Reset() => Multiplier = 1;
        protected abstract void Multiply(MasterBeat masterBeat);
        protected abstract void Divide(MasterBeat masterBeat);
    }
}
