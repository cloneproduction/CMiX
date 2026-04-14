// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using CMiX.Core.BaseControls;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public partial class MasterBeat : ReceivableControl, IControl
    {
        public MasterBeat(GenericValue<int> index, 
                          GenericValue<float> period, 
                          GenericValue<int> beatIndex, 
                          GenericValue<bool> pause, 
                          Button resync,
                          UndoManager undoManager,
                          ControlActivationService activationService)
        {
            Index = index;
            Period = period;
            BeatIndex = beatIndex;
            Pause = pause;
            Resync = resync;

            BeatIndex.Value = 0;
            Index.Value = 3;
            Period.Value = 1000;

            Periods = new float[15];
            tapPeriods = new List<float>();
            tapTime = new List<float>();

            BeatAnimations = new BeatAnimations();

            GeneratePeriods(Period.Value);
            BeatAnimations.MakeStoryBoard(Periods);
            SetAnimatedDouble();

            IsActive = false;
            UndoManager = undoManager;

            Period.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GenericValue<float>.Value))
                    OnPropertyChanged(nameof(BPM));
            };

            activationService.Register(this);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Pause { get; set; }
        public BeatAnimations BeatAnimations { get; set; }
        public Button Resync { get; set; }
        public GenericValue<int> Index { get; set; }
        public GenericValue<int> BeatIndex { get; set; }
        public GenericValue<float> Period { get; set; }

        private readonly List<float> tapPeriods;
        private readonly List<float> tapTime;

        private int maxIndex = 3;
        private int minIndex = -3;

        [ObservableProperty]
        private AnimatedDouble animatedDouble;

        public float[] Periods { get; set; }

        public float BPM
        {
            get => Period.Value > 0 ? (float)Math.Round(60000f / Period.Value, 2) : 0f;
            set
            {
                if (value <= 0) return;
                WithUndo(() =>
                {
                    float newPeriod = 60000f / value;
                    GeneratePeriods(newPeriod);
                    BeatAnimations.MakeStoryBoard(Periods);
                    int midPoint = (Periods.Length - 1) / 2;
                    Period.Value = newPeriod;
                    BeatIndex.Value = midPoint;
                    Index.Value = 0;
                    AnimatedDouble = BeatAnimations.AnimatedDoubles[midPoint];
                    OnPropertyChanged(nameof(AnimatedDouble));
                });
                OnPropertyChanged(nameof(BPM));
            }
        }

        private void SetAnimatedDouble()
        {
            int midIndex = Index.Value + (Periods.Length - 1) / 2;
            BeatIndex.Value = midIndex;
            Period.Value = Periods[midIndex];
            AnimatedDouble = BeatAnimations.AnimatedDoubles[midIndex];
            OnPropertyChanged(nameof(AnimatedDouble));
        }

        [RelayCommand]
        public void Multiply() => WithUndo(() =>
        {
            Index.Value = Math.Clamp(Index.Value - 1, minIndex, maxIndex);
            SetAnimatedDouble();
        });

        [RelayCommand]
        public void Divide() => WithUndo(() =>
        {
            Index.Value = Math.Clamp(Index.Value + 1, minIndex, maxIndex);
            SetAnimatedDouble();
        });

        [RelayCommand]
        public void Tap() => WithUndo(() =>
        {
            UpdatePeriods(GetMasterPeriod());
            Index.Value = 0;
            SetAnimatedDouble();
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
                sw.Restart(); // reset and start
                return 0f;    // no period yet
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
            Period.Value = basePeriod;

            if (basePeriod <= 0)
                return;

            GeneratePeriods(basePeriod);
            BeatAnimations.MakeStoryBoard(Periods);
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
            UndoManager?.BeginGroup();
            action();
            UndoManager?.EndGroup();
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
        }
    }
}
