﻿using CMiX.Models;
using CMiX.Services;
using System;
using System.Collections.ObjectModel;
using Memento;

namespace CMiX.ViewModels
{
    public class BeatModifier : Beat
    {
        #region CONSTRUCTORS
        public BeatModifier(string layername, ObservableCollection<OSCMessenger> oscmessengers, Beat beat, Mementor mementor) 
            : base (oscmessengers, mementor)
        {
            MessageAddress = String.Format("{0}{1}/", layername, nameof(BeatModifier));

            MasterBeat = beat;
            Multiplier = 1.0;
            ChanceToHit = new Slider(MessageAddress + nameof(ChanceToHit), oscmessengers, mementor);
            ChanceToHit.Amount = 1.0;
            beat.PeriodChanged += (s, newvalue) =>
            {
                OnPeriodChanged(Period);
                Notify(nameof(Period));
                Notify(nameof(BPM));
            };
        }
        #endregion

        #region PROPERTIES
        private Beat MasterBeat { get; }
        public Slider ChanceToHit { get; }

        public override double Period
        {
            get => MasterBeat.Period * Multiplier;
            set => throw new InvalidOperationException("Property is readonly. When binding, use Mode=OneWay.");
        }

        public override double Multiplier
        {
            get => base.Multiplier;
            set
            {
                
                if (Mementor != null)
                    Mementor.PropertyChange(this, "Multiplier");                   
                base.Multiplier = value;
                OnPeriodChanged(Period);
                Notify(nameof(Period));
                Notify(nameof(BPM));
                SendMessages(MessageAddress + nameof(Multiplier), Multiplier);
            }
        }
        #endregion

        #region MULTIPLY/DIVIDE
        protected override void Multiply()
        {
            Multiplier /= 2;
        }

        protected override void Divide()
        {
            Multiplier *= 2;
        }
        #endregion

        #region COPY/PASTE
        public void Copy(BeatModifierModel beatmodifiermodel)
        {
            beatmodifiermodel.MessageAddress = MessageAddress;
            ChanceToHit.Copy(beatmodifiermodel.ChanceToHit);
            beatmodifiermodel.Multiplier = Multiplier;
        }

        public void Paste(BeatModifierModel beatmodifiermodel)
        {
            DisabledMessages();

            MessageAddress = beatmodifiermodel.MessageAddress;
            ChanceToHit.Paste(beatmodifiermodel.ChanceToHit);
            Multiplier = beatmodifiermodel.Multiplier;

            EnabledMessages();
        }
        #endregion
    }
}