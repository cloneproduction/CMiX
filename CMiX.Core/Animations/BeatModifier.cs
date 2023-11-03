// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public class BeatModifier : ObservableRecipient, IControl
    {
        public BeatModifier(ControlMessenger controlMessenger)
        {
            BeatIndex = new IntegerValue(0, controlMessenger);
            ChanceToHit = new FloatValue(1);

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue ChanceToHit { get; set; }
        public IntegerValue BeatIndex { get; set; }

        private int maxIndex = 4;
        private int minIndex = -4;

        public void Reset()
        {
            BeatIndex.Value = 0;
        }

        public void Multiply()
        {
            if (BeatIndex.Value <= minIndex)
                return;
            BeatIndex.Value--;
        }

        public void Divide()
        {
            if (BeatIndex.Value >= maxIndex)
                return;
            BeatIndex.Value++;
        }
    }
}
