// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using CMiX.Core.BaseControls;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public partial class MasterBeat : ReceivableControl, IControl
    {
        public MasterBeat(GenericValue<int> index,
                          GenericValue<float> period,
                          GenericValue<int> beatIndex,
                          GenericValue<bool> pause,
                          CMiXButton resync,
                          UndoManager undoManager,
                          ControlActivationService activationService)
        {
            Index = index;
            Period = period;
            BeatIndex = beatIndex;
            Pause = pause;
            Resync = resync;


            Index.Value = 0;
            Period.Value = 1000;

            Periods = new float[15];
            tapPeriods = new List<float>();
            tapTime = new List<float>();

            GeneratePeriods(Period.Value);

            BeatIndex.Value = (Periods.Length - 1) / 2;
            IsActive = false;
            UndoManager = undoManager;

            Period.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GenericValue<float>.Value))
                    OnPropertyChanged(nameof(BPM));
            };

            activationService.Register(this);

            Index.Activate();
            Period.Activate();
            BeatIndex.Activate();
            Pause.Activate();
            Index.ID = ManagerIDs.Index;
            Period.ID = ManagerIDs.Period;
            BeatIndex.ID = ManagerIDs.BeatIndex;
            Pause.ID = ManagerIDs.Pause;
            Resync.ID = ManagerIDs.Resync;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Pause { get; set; }
        public CMiXButton Resync { get; set; }
        public GenericValue<int> Index { get; set; }
        public GenericValue<int> BeatIndex { get; set; }
        public GenericValue<float> Period { get; set; }
        public Func<int, IAnimatedDouble> AnimatedDoubleProvider { get; set; }


        private IAnimatedDouble _animatedDouble;
        public IAnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set
            {
                if (_animatedDouble != null)
                    _animatedDouble.PositionChanged -= OnAnimatedDoublePositionChanged;
                _animatedDouble = value;
                if (_animatedDouble != null)
                    _animatedDouble.PositionChanged += OnAnimatedDoublePositionChanged;
                OnPropertyChanged();
            }
        }

        // Cached because this runs on every beat clock tick and the string overload would allocate
        // a fresh event argument each time.
        private static readonly System.ComponentModel.PropertyChangedEventArgs AnimatedDoubleChangedArgs =
            new(nameof(AnimatedDouble));

        private void OnAnimatedDoublePositionChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(AnimatedDoubleChangedArgs);
        }

        private readonly List<float> tapPeriods;
        private readonly List<float> tapTime;

        private const int MaxIndex = 3;
        private const int MinIndex = -3;

        public float[] Periods { get; set; }

        public float BPM
        {
            get => BeatHelper.CalculateBPM(Period.Value);
            set
            {
                if (value <= 0) return;
                WithUndo(() =>
                {
                    Period.Value = 60000f / value;
                    Index.Value = 0;
                    ApplyPeriod(Period.Value);
                });
                OnPropertyChanged(nameof(BPM));
            }
        }

        private void ApplyPeriod(float period)
        {
            GeneratePeriods(period);
            OnPropertyChanged(nameof(BPM));
            OnPropertyChanged(nameof(Periods));
        }

        [RelayCommand]
        public void Multiply() => WithUndo(() =>
        {
            Index.Value = Math.Clamp(Index.Value - 1, MinIndex, MaxIndex);
            int midIndex = Index.Value + (Periods.Length - 1) / 2;
            BeatIndex.Value = midIndex;
            Period.Value = Periods[midIndex];
            OnPropertyChanged(nameof(BPM));
            OnPropertyChanged(nameof(Periods));
        });

        [RelayCommand]
        public void Divide() => WithUndo(() =>
        {
            Index.Value = Math.Clamp(Index.Value + 1, MinIndex, MaxIndex);
            int midIndex = Index.Value + (Periods.Length - 1) / 2;
            BeatIndex.Value = midIndex;
            Period.Value = Periods[midIndex];
            OnPropertyChanged(nameof(BPM));
            OnPropertyChanged(nameof(Periods));
        });


        [RelayCommand]
        public void Tap() => WithUndo(() =>
        {
            UpdatePeriods(GetMasterPeriod());
            Index.Value = 0;
        });

        Stopwatch sw = new Stopwatch();

        private float GetMasterPeriod()
        {
            if (!sw.IsRunning)
                sw.Start();

            float currentMs = sw.ElapsedMilliseconds;

            if (tapTime.Count > 0 && currentMs - tapTime.Last() > 5000)
            {
                tapTime.Clear();
                tapPeriods.Clear();
                sw.Restart();
                return 0f;
            }

            tapTime.Add(currentMs);

            if (tapTime.Count > 1)
            {
                tapPeriods.Clear();
                tapPeriods.AddRange(tapTime.Zip(tapTime.Skip(1), (prev, next) => next - prev));
            }

            return tapPeriods.Count > 0 ? tapPeriods.Average() : 0f;
        }

        private void UpdatePeriods(float basePeriod)
        {
            if (basePeriod <= 0) return;
            Period.Value = basePeriod;
            ApplyPeriod(basePeriod);
        }

        private void GeneratePeriods(float basePeriod)
        {
            float multiplier = 1f / 128f;
            for (int i = 0; i < Periods.Length; i++)
            {
                Periods[i] = multiplier * basePeriod;
                multiplier *= 2;
            }
        }

        private void WithUndo(Action action)
        {
            var before = ToModel();
            UndoManager?.SuppressUndo();
            action();
            UndoManager?.ResumeUndo();
            UndoManager?.Push(new ValueChangedCommand(this, before, ToModel()));
        }

        public IControlModel ToModel() => new MasterBeatModel
        {
            ID = ID,
            Resync = (ButtonModel)Resync.ToModel(),
            Pause = (GenericValueModel<bool>)Pause.ToModel(),
            Index = (GenericValueModel<int>)Index.ToModel(),
            BeatIndex = (GenericValueModel<int>)BeatIndex.ToModel(),
            Period = (GenericValueModel<float>)Period.ToModel(),
            Periods = Periods.ToArray()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MasterBeatModel)model;
            ID = m.ID;
            Resync.FromModel(m.Resync);
            Pause.FromModel(m.Pause);
            Index.FromModel(m.Index);
            BeatIndex.FromModel(m.BeatIndex);
            Period.FromModel(m.Period);
            Periods = m.Periods.ToArray();
            ApplyPeriod(Period.Value);
        }
    }
}
