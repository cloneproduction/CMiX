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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class MasterBeat : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public MasterBeat(MasterBeatModel masterBeatModel)
        {
            this.ID = masterBeatModel.ID;

            Index = 0;
            Period = 1000;
            Multiplier = 1;
            Periods = new double[15];
            tapPeriods = new List<double>();
            tapTime = new List<double>();

            BeatAnimations = new BeatAnimations();
            Resync = new Resync(BeatAnimations, masterBeatModel.ResyncModel);

            UpdatePeriods(Period);
            SetAnimatedDouble();

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
        }


        public Guid ID { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public double Multiplier { get; set; }

        public BeatAnimations BeatAnimations { get; set; }
        public Resync Resync { get; set; }


        private readonly List<double> tapPeriods;
        private readonly List<double> tapTime;


        private double CurrentTime => (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;

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


        private double _period;
        public double Period
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

        public void Reset() => Multiplier = 1;

        private void SetAnimatedDouble()
        {
            BeatIndex = Index + (Periods.Length - 1) / 2;
            Period = Periods[Index + (Periods.Length - 1) / 2];
            AnimatedDouble = BeatAnimations.AnimatedDoubles[Index + (Periods.Length - 1) / 2];
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

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
                return;
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
    }
}
