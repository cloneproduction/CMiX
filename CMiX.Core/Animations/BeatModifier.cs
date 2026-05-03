// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public class BeatModifier : ReceivableControl, IPrefab, IControl, IDisposable
    {
        public BeatModifier(PrefabService prefabService,
                            MasterBeat masterBeat,
                            GenericValue<int> beatIndex,
                            GenericValue<float> chanceToHit,
                            Easing easing,
                            UndoManager undoManager,
                            ControlActivationService activationService)
        {
            PrefabService = prefabService;
            BeatIndex = beatIndex;
            ChanceToHit = chanceToHit;
            Easing = easing;

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);

            UndoManager = undoManager;
            IsActive = false;
            activationService.Register(this);

            _onBPMChanged = (s, e) => OnPropertyChanged(nameof(BPM));
            BeatIndex.PropertyChanged += _onBPMChanged;

            MasterBeat = masterBeat;
        }

        private readonly System.ComponentModel.PropertyChangedEventHandler _onBPMChanged;

        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public Guid ID { get; set; } = Guid.NewGuid();
        public Easing Easing { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> ChanceToHit { get; set; }
        public GenericValue<int> BeatIndex { get; set; }

        private MasterBeat _masterBeat;
        public MasterBeat MasterBeat
        {
            get => _masterBeat;
            set
            {
                if (_masterBeat != null)
                {
                    _masterBeat.BeatIndex.PropertyChanged -= _onBPMChanged;
                    _masterBeat.PropertyChanged -= _onBPMChanged;
                }
                _masterBeat = value;
                if (_masterBeat != null)
                {
                    _masterBeat.BeatIndex.PropertyChanged += _onBPMChanged;
                    _masterBeat.PropertyChanged += _onBPMChanged;
                }
                OnPropertyChanged(nameof(BPM));
            }
        }

        public float BPM => BeatHelper.CalculateBPM(MasterBeat.Periods[BeatIndex.Value + MasterBeat.BeatIndex.Value]);

        private const int MaxIndex = 4;
        private const int MinIndex = -4;

        private void WithUndo(Action action)
        {
            var before = ToModel();
            UndoManager?.BeginGroup();
            action();
            UndoManager?.EndGroup();
            UndoManager?.Push(new ValueChangedCommand(this, before, ToModel()));
        }

        public void Reset() => WithUndo(() => BeatIndex.Value = 0);

        public void Multiply() => WithUndo(() =>
        {
            if (BeatIndex.Value > MinIndex)
                BeatIndex.Value--;
        });

        public void Divide() => WithUndo(() =>
        {
            if (BeatIndex.Value < MaxIndex)
                BeatIndex.Value++;
        });

        public void Dispose()
        {
            BeatIndex.PropertyChanged -= _onBPMChanged;
            if (_masterBeat != null)
            {
                _masterBeat.BeatIndex.PropertyChanged -= _onBPMChanged;
                _masterBeat.PropertyChanged -= _onBPMChanged;
            }
        }

        public IControlModel ToModel() => new BeatModifierModel
        {
            ID = ID,
            Easing = (EasingModel)Easing.ToModel(),
            BeatIndex = (GenericValueModel<int>)BeatIndex.ToModel(),
            ChanceToHit = (GenericValueModel<float>)ChanceToHit.ToModel(),
            PrefabService = (PrefabServiceModel)PrefabService.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BeatModifierModel)model;
            ID = m.ID;
            Easing.FromModel(m.Easing);
            BeatIndex.FromModel(m.BeatIndex);
            ChanceToHit.FromModel(m.ChanceToHit);
            PrefabService.FromModel(m.PrefabService);
        }
    }
}
