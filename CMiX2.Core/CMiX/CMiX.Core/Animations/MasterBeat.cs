// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Animations
{
    public partial class MasterBeat : ObservableRecipient
    {
        public MasterBeat(MasterBeatModel masterBeatModel)
        {
            ID = masterBeatModel.ID;

            Index = new IntegerValue(masterBeatModel.Index);
            Period = new FloatValue(masterBeatModel.Period);
            BeatIndex = new IntegerValue(masterBeatModel.BeatIndex);
            Pause = new BooleanValue(masterBeatModel.Pause);

            Periods = new float[15];
            tapPeriods = new List<float>();
            tapTime = new List<float>();

            BeatAnimations = new BeatAnimations();
            Resync = new Button(masterBeatModel.Resync);

            var Multiplier = 1.0f / 128.0f;
            for (var i = 0; i < Periods.Length; i++)
            {
                Periods[i] = Multiplier * Period.Value;
                Multiplier *= 2;
            }

            BeatAnimations.MakeStoryBoard(Periods);
            SetAnimatedDouble();

            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
            TapCommand = new RelayCommand(Tap);
        }

        public Guid ID { get; set; }
        public ICommand ResetCommand { get; }
        public ICommand MultiplyCommand { get; }
        public ICommand DivideCommand { get; }
        public ICommand TapCommand { get; }
        public BooleanValue Pause { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public BeatAnimations BeatAnimations { get; set; }
        public Button Resync { get; set; }
        public IntegerValue Index { get; set; }
        public IntegerValue BeatIndex { get; set; }
        public FloatValue Period { get; set; }

        private readonly List<float> tapPeriods;
        private readonly List<float> tapTime;

        private int maxIndex = 3;
        private int minIndex = -3;

        [ObservableProperty]
        private AnimatedDouble animatedDouble;


        private float[] _periods;
        public float[] Periods
        {
            get => _periods;
            set => SetProperty(ref _periods, value);
        }

        private void SetAnimatedDouble()
        {
            BeatIndex.Value = Index.Value + (Periods.Length - 1) / 2;
            Period.Value = Periods[Index.Value + (Periods.Length - 1) / 2];
            animatedDouble = BeatAnimations.AnimatedDoubles[Index.Value + (Periods.Length - 1) / 2];
            Console.WriteLine();
            //OnPropertyChanged("Period");
        }

        public void Multiply()
        {
            if (Index.Value <= minIndex)
                return;
            Index.Value--;
            SetAnimatedDouble();
        }

        public void Divide()
        {
            if (Index.Value >= maxIndex)
                return;
            Index.Value++;
            SetAnimatedDouble();
        }
        public void Tap()
        {
            UpdatePeriods(GetMasterPeriod());
            Index.Value = 0;
            SetAnimatedDouble();
            //OnPropertyChanged("Period");
        }

        Stopwatch sw = new Stopwatch();
        float ms = 0;

        private float GetMasterPeriod()
        {
            if (!sw.IsRunning)
                sw.Start();

            ms = sw.ElapsedMilliseconds;

            if (tapTime.Count > 1 && ms - tapTime[tapTime.Count - 1] > 5000)
            {
                tapTime.Clear();
                sw.Reset();
                sw.Start();
                return tapPeriods.Sum() / tapPeriods.Count;
            }

            tapTime.Add(ms);

            if (tapTime.Count > 1)
            {
                tapPeriods.Clear();
                for (var i = 1; i < tapTime.Count; i++)
                    tapPeriods.Add(tapTime[i] - tapTime[i - 1]);
            }

            return tapPeriods.Sum() / tapPeriods.Count;
        }

        private void UpdatePeriods(float period)
        {
            Period.Value = period;
            if (period > 0)
            {
                var Multiplier = 1.0f / 128.0f;
                for (var i = 0; i < Periods.Length; i++)
                {
                    Periods[i] = Multiplier * Period.Value;
                    Multiplier *= 2;
                }
                BeatAnimations.MakeStoryBoard(Periods);
            }
        }
    }
}
