// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.Controls;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatModifier : Beat, IControl, IBeatObserver
    {
        public BeatModifier(MasterBeat masterBeat, BeatModifierModel beatModifierModel)
            : base(beatModifierModel)
        {
            this.ID = beatModifierModel.ID;

            ChanceToHit = new Slider(nameof(ChanceToHit), beatModifierModel.ChanceToHit) { Minimum = 0, Maximum = 100 };
            Multiplier = beatModifierModel.Multiplier;

            MasterBeat = masterBeat;
            MasterBeat.Attach(this);
            Period = masterBeat.Period;

            SetAnimatedDouble();
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public Slider ChanceToHit { get; set; }


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
            set => SetProperty(ref _period, value);
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

        private void SetAnimatedDouble()
        {
            BeatIndex = Index + MasterBeat.BeatIndex;
            Period = MasterBeat.Periods[Index + MasterBeat.BeatIndex];
            AnimatedDouble = MasterBeat.BeatAnimations.AnimatedDoubles[Index + MasterBeat.BeatIndex];
            //Notify(nameof(BPM));
            Communicator?.SendMessage(new MessageUpdateViewModel(this));
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

        public void UpdatePeriod(double period)
        {
            SetAnimatedDouble();
        }

        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }
    }
}
