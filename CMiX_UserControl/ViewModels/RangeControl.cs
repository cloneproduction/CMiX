﻿using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using System.Collections.ObjectModel;
using Memento;

namespace CMiX.ViewModels
{
    public class RangeControl : SendableViewModel
    {
        #region CONSTRUCTORS
        public RangeControl(string messageaddress, ObservableCollection<ServerValidation> serverValidations, Mementor mementor) 
            : base (serverValidations, mementor)
        {
            MessageAddress = messageaddress + "/";
            Range = new Slider(MessageAddress + nameof(Range), serverValidations, mementor);
            Modifier = ((RangeModifier)0).ToString();
        }
        #endregion

        #region METHODS
        public void UpdateMessageAddress(string messageaddress)
        {
            MessageAddress = messageaddress;
            Range.UpdateMessageAddress(messageaddress + nameof(Range) + "/");
        }
        #endregion

        #region PROPERTIES
        public Slider Range { get; }

        private string _modifier;
        public string Modifier
        {
            get => _modifier;
            set
            {
                if(Mementor != null)
                    Mementor.PropertyChange(this, nameof(Modifier));
                SetAndNotify(ref _modifier, value);
                //SendMessages(MessageAddress + nameof(Modifier), Modifier);
            }
        }
        #endregion

        #region COPY/PASTE
        public void Copy(RangeControlModel rangecontrolmodel)
        {
            rangecontrolmodel.MessageAddress = MessageAddress;
            Range.Copy(rangecontrolmodel.Range);
            rangecontrolmodel.Modifier = Modifier;
        }

        public void Paste(RangeControlModel rangecontrolmodel)
        {
            DisabledMessages();

            MessageAddress = rangecontrolmodel.MessageAddress;
            Range.Paste(rangecontrolmodel.Range);
            Modifier = rangecontrolmodel.Modifier;

            EnabledMessages();
        }

        public void Reset()
        {
            DisabledMessages();
            Modifier = ((RangeModifier)0).ToString();
            Range.Reset();
            EnabledMessages();
        }
        #endregion
    }
}