// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Controls;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Network;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.Beat
{
    public class MasterBeat : ObservableRecipient, IRecipient<MessageRequestControl>, IPrefab
    {
        public MasterBeat(MasterBeatModel masterBeatModel, CompositionService compositionService)
        {
            this.ID = masterBeatModel.ID;
            CompositionService = compositionService;

            Index = 0;
            Period = 1000;
            Periods = new float[15];
            tapPeriods = new List<float>();
            tapTime = new List<float>();

            BeatAnimations = new BeatAnimations();
            Resync = new Resync(BeatAnimations, masterBeatModel.ResyncModel);
            Pause = new BooleanValue(masterBeatModel.Pause);

            UpdatePeriods(Period);
            SetAnimatedDouble();

            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
            TapCommand = new RelayCommand(Tap);

            IsActive = true;
        }


        public Guid ID { get; set; }
        public ICommand ResetCommand { get;  }
        public ICommand MultiplyCommand { get;  }
        public ICommand DivideCommand { get;}
        public ICommand TapCommand { get; }
        public CompositionService CompositionService { get; set; }
        public BooleanValue Pause { get; set; }


        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public BeatAnimations BeatAnimations { get; set; }
        public Resync Resync { get; set; }


        private readonly List<float> tapPeriods;
        private readonly List<float> tapTime;

        private int maxIndex = 3;
        private int minIndex = -3;


        private AnimatedDouble _animatedDouble;
        public AnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set => SetProperty(ref _animatedDouble, value);
        }

        public int Index { get; set; }
        public int BeatIndex { get; set; }


        private float _period;
        public float Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }


        private float[] _periods;
        public float[] Periods
        {
            get => _periods;
            set => SetProperty(ref _periods, value);
        }


        private void SetAnimatedDouble()
        {
            BeatIndex = Index + (Periods.Length - 1) / 2;
            Period = Periods[Index + (Periods.Length - 1) / 2];
            AnimatedDouble = BeatAnimations.AnimatedDoubles[Index + (Periods.Length - 1) / 2];
            OnPropertyChanged("Period");
            ControlMessenger.Send<MasterBeatModel>(this);
        }


        public void Multiply()
        {
            if (Index <= minIndex)
                return;
            Index--;
            SetAnimatedDouble();
        }

        public void Divide()
        {
            if (Index >= maxIndex)
                return;
            Index++;
            SetAnimatedDouble();
        }

        public void Tap()
        {
            UpdatePeriods(GetMasterPeriod());
            Index = 0;
            SetAnimatedDouble();
            OnPropertyChanged("Period");
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
                for (int i = 1; i < tapTime.Count; i++)
                    tapPeriods.Add(tapTime[i] - tapTime[i - 1]);
            }

            return tapPeriods.Sum() / tapPeriods.Count;
        }


        private void UpdatePeriods(float period)
        {
            Period = period;
            if (period > 0)
            {
                float Multiplier = 1.0f / 128.0f;
                for (int i = 0; i < Periods.Length; i++)
                {
                    Periods[i] = Multiplier * Period;
                    Multiplier *= 2;
                }
                BeatAnimations.MakeStoryBoard(Periods);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
