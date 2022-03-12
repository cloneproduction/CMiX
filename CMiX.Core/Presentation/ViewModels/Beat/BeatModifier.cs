// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.Controls;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatModifier : ObservableRecipient, 
        IRecipient<IMessage>,
        IControl, 
        IBeatable,
        IDisposable
    {
        public BeatModifier(BeatModifierModel beatModifierModel)
        {
            this.ID = beatModifierModel.ID;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);

            ChanceToHit = new Slider(nameof(ChanceToHit), beatModifierModel.ChanceToHit) { Minimum = 0, Maximum = 100 };
            SetAnimatedDouble();

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public MasterBeat MasterBeat { get; set; }

        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }

        public Guid ID { get; set; }
        public Slider ChanceToHit { get; set; }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            if(MasterBeat != null)
            {
                if(masterBeat!= this.MasterBeat)
                {
                    MasterBeat.UnregisterBeatModifier(this);
                    masterBeat.RegisterBeatModifier(this);
                }
                SetAnimatedDouble();
            }

            MasterBeat = masterBeat;
            masterBeat.RegisterBeatModifier(this);
            SetAnimatedDouble();
        }


        private AnimatedDouble _animatedDouble;
        public AnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set => SetProperty(ref _animatedDouble, value);
        }


        private int maxIndex = 4;
        private int minIndex = -4;


        private float _period;
        public float Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        private int _beatIndex;
        public int BeatIndex
        {
            get => _beatIndex;
            set => _beatIndex = value;
        }


        public void Reset()
        {
            BeatIndex = 0;
            SetAnimatedDouble();
        }


        public void Multiply()
        {
            if (BeatIndex <= minIndex)
                return;
            BeatIndex--;
            SetAnimatedDouble();
        }

        public void Divide()
        {
            if (BeatIndex >= maxIndex)
                return;
            BeatIndex++;
            SetAnimatedDouble();
        }


        public void SetAnimatedDouble()
        {
            if (MasterBeat != null)
            {
                Period = MasterBeat.Periods[BeatIndex + MasterBeat.BeatIndex];
                AnimatedDouble = MasterBeat.BeatAnimations.AnimatedDoubles[BeatIndex + MasterBeat.BeatIndex];
            }
            else
            {
                Period = 0;
                AnimatedDouble = null;
            }

            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
        }

        public void SetViewModel(IModel model)
        {
            BeatModifierModel beatModifierModel = model as BeatModifierModel;
            this.ID = beatModifierModel.ID;
            this.BeatIndex = beatModifierModel.BeatIndex;
            this.Period = beatModifierModel.Period;
            this.ChanceToHit.SetViewModel(beatModifierModel.ChanceToHit);
        }

        public IModel GetModel()
        {
            BeatModifierModel model = new BeatModifierModel();
            model.ID = this.ID;
            model.Period = this.Period;
            model.BeatIndex = this.BeatIndex;
            model.ChanceToHit = (SliderModel)this.ChanceToHit.GetModel();
            return model;
        }


        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if (message is MessageUpdateViewModel msg)
            {
                this.SetViewModel(msg.Model);
                SetAnimatedDouble();
            }
        }

        public void Dispose()
        {
            MasterBeat?.UnregisterBeatModifier(this);
        }
    }
}
