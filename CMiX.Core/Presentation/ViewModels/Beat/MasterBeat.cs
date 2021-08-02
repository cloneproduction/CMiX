// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.Controls;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class MasterBeat : Beat, IRecipient<IMessage>, IControl, IBeatSubject
    {
        public MasterBeat(MasterBeatModel masterBeatModel) : base(masterBeatModel)
        {
            this.ID = masterBeatModel.ID;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            BeatObservers = new List<IBeatObserver>();
            Index = 0;
            Period = 1000;
            Multiplier = 1;
            Periods = new double[15];

            BeatAnimations = new BeatAnimations();
            Resync = new Resync(BeatAnimations, masterBeatModel.ResyncModel);

            UpdatePeriods(Period);
            SetAnimatedDouble();

            tapPeriods = new List<double>();
            tapTime = new List<double>();

            TapCommand = new RelayCommand(Tap);
        }

        public Guid ID { get; set; }
        public BeatAnimations BeatAnimations { get; set; }
        public Resync Resync { get; set; }
        public ICommand TapCommand { get; }


        private readonly List<double> tapPeriods;
        private readonly List<double> tapTime;

        private double CurrentTime => (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;

        private int maxIndex = 3;
        private int minIndex = -3;

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                this.NotifyBeatChange(Period);
            }
        }

        private int _beatIndex;
        public int BeatIndex
        {
            get => _beatIndex;
            set => _beatIndex = value;
        }

        private double _period;
        public override double Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        private double[] _periods;
        public double[] Periods
        {
            get => _periods;
            set => SetProperty(ref _periods, value);
        }

        private AnimatedDouble _animatedDouble;
        public AnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set => SetProperty(ref _animatedDouble, value);
        }


        public event EventHandler IndexChanged;
        protected void OnIndexChanged() => IndexChanged?.Invoke(this, null);



        private void SetAnimatedDouble()
        {
            BeatIndex = Index + (Periods.Length - 1) / 2;
            Period = Periods[Index + (Periods.Length - 1) / 2];
            AnimatedDouble = BeatAnimations.AnimatedDoubles[Index + (Periods.Length - 1) / 2];
            this.NotifyBeatChange(Period);
            OnPropertyChanged(nameof(BPM));
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
        }


        protected override void Multiply()
        {
            if (Index <= minIndex)
                return;
            Index--;
            SetAnimatedDouble();
        }

        protected override void Divide()
        {
            if (Index >= maxIndex)
                return;
            Index++;
            SetAnimatedDouble();
        }

        private void Tap()
        {
            UpdatePeriods(GetMasterPeriod());
            Index = 0;
            SetAnimatedDouble();
        }


        private double GetMasterPeriod()
        {
            double ms = CurrentTime;

            if (tapTime.Count > 1 && ms - tapTime[tapTime.Count - 1] > 5000)
                tapTime.Clear();

            tapTime.Add(ms);

            if (tapTime.Count > 1)
            {
                tapPeriods.Clear();
                for (int i = 1; i < tapTime.Count; i++)
                    tapPeriods.Add(tapTime[i] - tapTime[i - 1]);
            }
            return tapPeriods.Sum() / tapPeriods.Count;
        }


        private void UpdatePeriods(double period)
        {
            Period = period;
            if (period > 0)
            {
                double Multiplier = 1.0 / 128.0;
                for (int i = 0; i < Periods.Length; i++)
                {
                    Periods[i] = Multiplier * Period;
                    Multiplier *= 2;
                }
                BeatAnimations.MakeStoryBoard(Periods);
            }
        }

        private List<IBeatObserver> BeatObservers { get; set; }

        public void Attach(IBeatObserver observer)
        {
            BeatObservers.Add(observer);
        }

        public void Detach(IBeatObserver observer)
        {
            BeatObservers.Remove(observer);
        }

        public void NotifyBeatChange(double period)
        {
            foreach (var observer in BeatObservers)
            {
                observer.UpdatePeriod(period);
            }
        }

        public void SetViewModel(IModel model)
        {
            MasterBeatModel masterBeatModel = model as MasterBeatModel;
            this.ID = masterBeatModel.ID;
            this.Period = masterBeatModel.Period;
            this.Periods = masterBeatModel.Periods;
            this.Multiplier = masterBeatModel.Multiplier;
        }

        public IModel GetModel()
        {
            MasterBeatModel model = new MasterBeatModel();
            model.ID = this.ID;
            model.Period = this.Period;
            model.Periods = this.Periods;
            model.Multiplier = this.Multiplier;
            return model;
        }

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
            }
        }
    }
}
