﻿using CMiX.MVVM.ViewModels;
using System;
using Memento;
using CMiX.MVVM.Services;

namespace CMiX.Studio.ViewModels
{
    public class BeatModifier : Beat, ISendable, IUndoable
    {
        #region CONSTRUCTORS
        public BeatModifier(string messageAddress, Beat beat, MessageService messageService, Mementor mementor)
        {
            MessageAddress = $"{messageAddress}{nameof(BeatModifier)}/";
            MessageService = messageService;

            Beat = beat;
            Multiplier = 1.0;
            ChanceToHit = new Slider(MessageAddress + nameof(ChanceToHit), messageService, mementor)
            {
                Amount = 1.0
            };
            beat.PeriodChanged += (s, newvalue) =>
            {
                OnPeriodChanged(Period);
                Notify(nameof(Period));
                Notify(nameof(BPM));
            };
        }
        #endregion

        #region PROPERTIES
        private Beat Beat { get; }
        public Slider ChanceToHit { get; }

        public override double Period
        {
            get => Beat.Period * Multiplier;
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
                //SendMessages(MessageAddress + nameof(Multiplier), Multiplier);
            }
        }

        public string MessageAddress { get; set; }
        public Mementor Mementor { get; set; }
        public MessageService MessageService { get; set; }
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

        #region COPY/PASTE/RESET

        public void Reset()
        {
            MessageService.Disable();

            Multiplier = 1.0;
            ChanceToHit.Reset();
            ChanceToHit.Amount = 1.0;

            MessageService.Enable();
        }
        #endregion
    }
}