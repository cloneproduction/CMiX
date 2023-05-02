// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public class BeatModifier : ObservableRecipient, IControl, IDisposable
    {
        public BeatModifier(BeatModifierModel beatModifierModel)
        {
            ID = beatModifierModel.ID;
            BeatIndex = new IntegerValue(beatModifierModel.BeatIndex);
            ChanceToHit = new FloatValue(beatModifierModel.ChanceToHit);
            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }

        public Guid ID { get; set; }
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

        public void Dispose()
        {

        }
    }
}
