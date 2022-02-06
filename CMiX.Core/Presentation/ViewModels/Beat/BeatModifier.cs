// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        IRecipient<MessageMasterBeatCollectionChanged>,
        IRecipient<MessageSelectedMasterBeatChange>,
        IRecipient<MessageMasterBeatChange>,
        IControl
    {
        public BeatModifier(BeatModifierModel beatModifierModel)
        {
            this.ID = beatModifierModel.ID;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);

            ChanceToHit = new Slider(nameof(ChanceToHit), beatModifierModel.ChanceToHit) { Minimum = 0, Maximum = 100 };
            Multiplier = beatModifierModel.Multiplier;

            MasterBeats = new ObservableCollection<MasterBeat>();

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public BeatModifier(BeatModifierModel beatModifierModel, Guid componentID) : this(beatModifierModel)
        {
            WeakReferenceMessenger.Default.RegisterAll(this, componentID);
            SelectedMasterBeat = WeakReferenceMessenger.Default.Send(new MessageRequestMasterBeat(), componentID).Response;
            SetAnimatedDouble();
        }


        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public double Multiplier { get; set; }

        public Guid ID { get; set; }
        public Slider ChanceToHit { get; set; }
        public MasterBeat SelectedMasterBeat { get; set; }



        private AnimatedDouble _animatedDouble;
        public AnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set => SetProperty(ref _animatedDouble, value);
        }


        private int maxIndex = 4;
        private int minIndex = -4;


        private double _period;
        public double Period
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


        public bool CheckHitOnBeatTick()
        {
            return (MathUtils.RandomDouble(0.0, 1.0) <= this.ChanceToHit.Amount / this.ChanceToHit.Maximum) ? true : false;
        }


        public void Reset()
        {
            BeatIndex = 0;
            SetAnimatedDouble();
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
        }


        public void Multiply()
        {
            if (BeatIndex <= minIndex)
                return;
            BeatIndex--;
            SetAnimatedDouble();
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
        }

        public void Divide()
        {
            if (BeatIndex >= maxIndex)
                return;
            BeatIndex++;
            SetAnimatedDouble();
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
        }


        private void SetAnimatedDouble()
        {
            if (SelectedMasterBeat != null)
            {
                Period = SelectedMasterBeat.Periods[BeatIndex + SelectedMasterBeat.BeatIndex];
                AnimatedDouble = SelectedMasterBeat.BeatAnimations.AnimatedDoubles[BeatIndex + SelectedMasterBeat.BeatIndex];
                return;
            }

            Period = 0;
            AnimatedDouble = null;
        }

        public void SetViewModel(IModel model)
        {
            BeatModifierModel beatModifierModel = model as BeatModifierModel;
            this.ID = beatModifierModel.ID;
            this.BeatIndex = beatModifierModel.BeatIndex;
            this.Multiplier = beatModifierModel.Multiplier;
            this.Period = beatModifierModel.Period;
            this.ChanceToHit.SetViewModel(beatModifierModel.ChanceToHit);
        }

        public IModel GetModel()
        {
            BeatModifierModel model = new BeatModifierModel();
            model.ID = this.ID;
            model.Period = this.Period;
            model.Multiplier = this.Multiplier;
            model.BeatIndex = this.BeatIndex;
            model.ChanceToHit = (SliderModel)this.ChanceToHit.GetModel();
            model.Multiplier = this.Multiplier;
            return model;
        }


        public ObservableCollection<MasterBeat> MasterBeats { get; set; }


        public void Receive(MessageMasterBeatCollectionChanged message)
        {
            MasterBeats = message.MasterBeats;

            if(MasterBeats.Count == 0)
                SelectedMasterBeat = null;

            if(SelectedMasterBeat != null)
            {
                if (!this.MasterBeats.Any(x => x.ID == this.SelectedMasterBeat.ID))
                {
                    SelectedMasterBeat = MasterBeats[0];
                }
            }

            SetAnimatedDouble();
        }

        public void Receive(MessageMasterBeatChange message)
        {
            if (SelectedMasterBeat == null)
                return;

            if(SelectedMasterBeat != null)
            {
                if (SelectedMasterBeat.ID == message.ID)
                {
                    SelectedMasterBeat.SetViewModel(message.MasterBeatModel);
                }
            }

            SetAnimatedDouble();
        }

        public void Receive(MessageSelectedMasterBeatChange messageSelectedMasterBeatChange)
        {
            //this.SelectedMasterBeat = MasterBeats.FirstOrDefault(x => x.ID == messageSelectedMasterBeatChange.MasterBeatID);
            //SetAnimatedDouble();
        }


        public void Receive(IMessage message)
        {
            if (message is MessageMasterBeatChange messageMasterBeatChange)
            {
                this.Receive(messageMasterBeatChange);
            }

            //if (message.ID != this.ID)
            //    return;

            if (message is MessageUpdateViewModel msg)
            {
                this.SetViewModel(msg.Model);
                SetAnimatedDouble();
            }

            if(message is MessageSelectedMasterBeatChange msgSelectedMasterBeatChange)
            {
                this.SelectedMasterBeat = MasterBeats.FirstOrDefault(x => x.ID == msgSelectedMasterBeatChange.MasterBeatID);
                SetAnimatedDouble();
            }

        }
    }
}
