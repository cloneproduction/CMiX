// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public class BeatModifier : ObservableRecipient, IPrefab, IControl
    {
        public BeatModifier(PrefabService prefabService,
                            MasterBeat masterBeat, 
                            GenericValue<int> beatIndex, 
                            GenericValue<float> chanceToHit,
                            Easing easing)
        {
            PrefabService = prefabService;
            BeatIndex = beatIndex;
            ChanceToHit = chanceToHit;
            MasterBeat = masterBeat;
            Easing = easing;

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public IControlModel ToModel() => this.ToModel();
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }

        public Guid ID { get; set; } = Guid.NewGuid();
        public MasterBeat MasterBeat { get; set; }
        public Easing Easing { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> ChanceToHit { get; set; }
        public GenericValue<int> BeatIndex { get; set; }


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
