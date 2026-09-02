// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation.Modulators
{
    public class BeatModifier : ReceivableControl, IDisposable, IModulator
    {
        public BeatModifier(PrefabService prefabService,
                            MasterBeat masterBeat,
                            GenericValue<int> beatIndex,
                            GenericValue<float> chanceToHit,
                            Easing easing,
                            BeatSteps beatSteps,
                            UndoManager undoManager,
                            ControlActivationService activationService)
        {
            PrefabService = prefabService;
            BeatIndex = beatIndex;
            ChanceToHit = chanceToHit;
            Easing = easing;
            BeatSteps = beatSteps;

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);

            UndoManager = undoManager;
            IsActive = false;
            activationService.Register(this);

            // Shared by three subscriptions: this modifier's own BeatIndex, MasterBeat.BeatIndex and
            // MasterBeat itself. The first two only ever raise Value; MasterBeat also re raises
            // AnimatedDouble on every clock tick (its own PositionChanged relay) with no table
            // change involved, so that name is filtered out here rather than re resolving and
            // reassigning AnimatedDouble sixty times a second for no reason. A real table rebuild
            // always announces Periods on its way through ApplyPeriod, so that name still passes.
            _onBPMChanged = (s, e) =>
            {
                if (e.PropertyName != nameof(GenericValue<int>.Value) && e.PropertyName != nameof(MasterBeat.Periods))
                    return;

                OnPropertyChanged(nameof(BPM));
                UpdateAnimatedDouble();
            };
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
        public BeatSteps BeatSteps { get; set; }

        private bool _isHovered;
        public bool IsHovered
        {
            get => _isHovered;
            set { _isHovered = value; OnPropertyChanged(); }
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<string> OutputNames { get; } = new[] { "Value" };

        // Randomizes around whatever base a bound field already has - the same "combine, don't
        // replace" behavior every existing binding already assumed before ModulatorKind existed.
        public ModulatorKind Kind => ModulatorKind.Modulate;


        private void OnResync(object sender, EventArgs e)
        {
            BeatSteps.CurrentStepIndex = 0;
            _stepAdvanced = true;
        }


        private bool _stepAdvanced;

        private void OnBeatPulse(object sender, EventArgs e)
        {
            if (_animatedDouble == null) return;
            double current = _animatedDouble.AnimationPosition;

            if (current >= 0.5 && !_stepAdvanced)
            {
                BeatSteps.CurrentStepIndex++;
                _stepAdvanced = true;
            }
            else if (current < 0.5)
            {
                _stepAdvanced = false;
            }
        }

        private IAnimatedDouble _animatedDouble;
        public IAnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set
            {
                if (_animatedDouble != null)
                    _animatedDouble.PositionChanged -= OnBeatPulse;
                _animatedDouble = value;
                if (_animatedDouble != null)
                    _animatedDouble.PositionChanged += OnBeatPulse;
                OnPropertyChanged();
            }
        }

        private void UpdateAnimatedDouble()
        {
            if (_masterBeat?.AnimatedDoubleProvider == null) return;
            int index = BeatIndex.Value + _masterBeat.BeatIndex.Value;
            var resolved = _masterBeat.AnimatedDoubleProvider(index);

            // The AnimatedDouble setter unsubscribes and resubscribes PositionChanged even when the
            // incoming value is reference equal to what is already held, so this only reassigns when
            // the resolved instance actually changed, rather than on every call this method makes.
            if (ReferenceEquals(resolved, _animatedDouble)) return;
            AnimatedDouble = resolved;
        }

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
                    _masterBeat.Resync.Click -= OnResync;
                }
                _masterBeat = value;
                if (_masterBeat != null)
                {
                    _masterBeat.BeatIndex.PropertyChanged += _onBPMChanged;
                    _masterBeat.PropertyChanged += _onBPMChanged;
                    _masterBeat.Resync.Click += OnResync;
                    UpdateAnimatedDouble();
                }
                OnPropertyChanged(nameof(BPM));
            }
        }

        public float BPM
        {
            get
            {
                var periods = MasterBeat.Periods;
                var index = Math.Clamp(BeatIndex.Value + MasterBeat.BeatIndex.Value, 0, periods.Length - 1);
                return BeatHelper.CalculateBPM(periods[index]);
            }
        }

        private const int MaxIndex = 4;
        private const int MinIndex = -4;

        private void WithUndo(Action action)
        {
            var before = ToModel();

            // Suppression is global, so a throwing action must not leave undo recording off.
            UndoManager?.SuppressUndo();
            try
            {
                action();
            }
            finally
            {
                UndoManager?.ResumeUndo();
            }

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
                _masterBeat.Resync.Click -= OnResync;
            }
            AnimatedDouble = null;
        }

        public IControlModel ToModel() => new BeatModifierModel
        {
            ID = ID,
            Easing = (EasingModel)Easing.ToModel(),
            BeatIndex = (GenericValueModel<int>)BeatIndex.ToModel(),
            ChanceToHit = (GenericValueModel<float>)ChanceToHit.ToModel(),
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatSteps = (BeatStepsModel)BeatSteps.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BeatModifierModel)model;
            ID = m.ID;
            Easing.FromModel(m.Easing);
            BeatIndex.FromModel(m.BeatIndex);
            ChanceToHit.FromModel(m.ChanceToHit);
            PrefabService.FromModel(m.PrefabService);
            BeatSteps.FromModel(m.BeatSteps);
        }
    }
}
