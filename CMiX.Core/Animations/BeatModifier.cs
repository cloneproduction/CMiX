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
                            GenericValue<float> chanceToHit)
        {
            PrefabService = prefabService;
            BeatIndex = beatIndex;
            ChanceToHit = chanceToHit;
            MasterBeat = masterBeat;

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }

        public Guid ID { get; set; } = Guid.NewGuid();
        public MasterBeat MasterBeat { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> ChanceToHit { get; set; }
        public GenericValue<int> BeatIndex { get; set; }


        private int maxIndex = 4;
        private int minIndex = -4;


        private float _currentPeriod;
        public float CurrentPeriods
        {
            get => _currentPeriod;
            set => SetProperty(ref _currentPeriod, value);
        }


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
