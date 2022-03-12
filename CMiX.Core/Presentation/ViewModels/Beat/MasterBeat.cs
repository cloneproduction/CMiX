// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Periods = new float[15];
            tapPeriods = new List<float>();
            tapTime = new List<float>();

            BeatAnimations = new BeatAnimations();
            Resync = new Resync(BeatAnimations, masterBeatModel.ResyncModel);
            BeatModifiers = new List<BeatModifier>();

            UpdatePeriods(Period);
            SetAnimatedDouble();

            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
        }


        public Guid ID { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }

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


        public float[] Periods { get; set; }

        private void SetAnimatedDouble()
        {
            BeatIndex = Index + (Periods.Length - 1) / 2;
            Period = Periods[Index + (Periods.Length - 1) / 2];
            AnimatedDouble = BeatAnimations.AnimatedDoubles[Index + (Periods.Length - 1) / 2];
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            UpdateBeatModifiers();
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

        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if (message is MessageUpdateViewModel msg)
            {
                this.SetViewModel(msg.Model);
                UpdateBeatModifiers();
            }
        }


        public void SetViewModel(IModel model)
        {
            MasterBeatModel masterBeatModel = model as MasterBeatModel;
            this.ID = masterBeatModel.ID;
            this.Period = masterBeatModel.Period;
            this.Periods = masterBeatModel.Periods;
            this.BeatIndex = masterBeatModel.BeatIndex;
            Resync.SetViewModel(masterBeatModel.ResyncModel);
        }

        public IModel GetModel()
        {
            MasterBeatModel model = new MasterBeatModel();
            model.ID = this.ID;
            model.Period = this.Period;
            model.Periods = this.Periods;
            model.BeatIndex = this.BeatIndex;
            model.ResyncModel = (ResyncModel)this.Resync.GetModel();

            return model;
        }

        public List<BeatModifier> BeatModifiers {get; set;}

        public void RegisterBeatModifier(BeatModifier beatModifier)
        {
            BeatModifiers.Add(beatModifier);
        }

        public void UnregisterBeatModifier(BeatModifier beatModifier)
        {
            BeatModifiers.Remove(beatModifier);
        }

        public void UpdateBeatModifiers()
        {

            foreach (var beatModifier in BeatModifiers)
            {
                beatModifier.SetAnimatedDouble();
            }
        }
    }
}
