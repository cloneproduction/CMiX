// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.Controls;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatModifier : Beat, IRecipient<IMessage>, IRecipient<MessageMasterBeatChange>, IRecipient<MessageSelectedMasterBeatChange>, IControl
    {
        public BeatModifier(BeatModifierModel beatModifierModel) : base(beatModifierModel)
        {
            this.ID = beatModifierModel.ID;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);

            ChanceToHit = new Slider(nameof(ChanceToHit), beatModifierModel.ChanceToHit) { Minimum = 0, Maximum = 100 };
            Multiplier = beatModifierModel.Multiplier;

            //MasterBeat = WeakReferenceMessenger.Default.Send(new MessageRequestMasterBeat(), MessageType.Internal).Response;
            //SetAnimatedDouble();
        }

        public BeatModifier(BeatModifierModel beatModifierModel, Guid componentID) : this(beatModifierModel)
        {
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal + componentID.ToString());
        }

        public Guid ID { get; set; }
        public Slider ChanceToHit { get; set; }
        public MasterBeat MasterBeat { get; set; }


        private int maxIndex = 4;
        private int minIndex = -4;

        private int _index = 0;
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        private double _period;
        public override double Period
        {
            get => _period;
            set
            {
                SetProperty(ref _period, value);
                OnPropertyChanged(nameof(BPM));
            }
        }

        private int _beatIndex;
        public int BeatIndex
        {
            get => _beatIndex;
            set => _beatIndex = value;
        }

        private AnimatedDouble _animatedDouble;
        public AnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set => SetProperty(ref _animatedDouble, value);
        }


        public bool CheckHitOnBeatTick()
        {
            return (MathUtils.RandomDouble(0.0, 1.0) <= this.ChanceToHit.Amount / this.ChanceToHit.Maximum) ? true : false;
        }

        public override void Multiply()
        {
            if (Index <= minIndex)
                return;
            Index--;
            SetAnimatedDouble();
        }

        public override void Divide()
        {
            if (Index >= maxIndex)
                return;
            Index++;
            SetAnimatedDouble();
        }

        private void SetAnimatedDouble()
        {
            if (MasterBeat != null)
            {
                BeatIndex = Index + MasterBeat.BeatIndex;
                Period = MasterBeat.Periods[Index + MasterBeat.BeatIndex];
                AnimatedDouble = MasterBeat.BeatAnimations.AnimatedDoubles[Index + MasterBeat.BeatIndex];
            }
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

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
                return;
            }
        }

        public void Receive(MessageMasterBeatChange message)
        {
            if (MasterBeat == null)
                return;

            if (this.MasterBeat.ID == message.MasterBeat.ID)
            {
                this.MasterBeat = message.MasterBeat;
                SetAnimatedDouble();
            }
        }

        public void Receive(MessageSelectedMasterBeatChange message)
        {
            if (MasterBeat == null)
            {
                this.MasterBeat = message.NewMasterBeat;
                SetAnimatedDouble();
                return;
            }


            if (message.OldMasterBeat != null)
            {
                if (this.MasterBeat.ID == message.OldMasterBeat.ID)
                {
                    this.MasterBeat = message.NewMasterBeat;
                    SetAnimatedDouble();

                }
            }


        }
    }
}
