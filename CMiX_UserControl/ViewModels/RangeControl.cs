﻿using System;
using CMiX.Services;
using CMiX.Models;
using GuiLabs.Undo;

namespace CMiX.ViewModels
{
    public class RangeControl : ViewModel, IMessengerData
    {
        #region CONSTRUCTORS
        public RangeControl(IMessenger messenger, string layername, ActionManager actionmanager)
        : this(
            actionmanager: actionmanager,
            range: new Slider(layername, messenger, actionmanager),
            modifier: ((RangeModifier)0).ToString(),
            messenger: messenger,
            messageaddress: String.Format("{0}/", layername),
            messageEnabled: true
          )
        { }

        public RangeControl
            (
                ActionManager actionmanager,
                Slider range,
                string modifier,
                IMessenger messenger,
                string messageaddress,
                bool messageEnabled
            )
            : base(actionmanager)
        {
            Range = range ?? throw new ArgumentNullException(nameof(range));
            Modifier = modifier;
            Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            MessageAddress = messageaddress;
            MessageEnabled = messageEnabled;
        }
        #endregion

        #region PROPERTIES
        public string MessageAddress { get; set; }
        public bool MessageEnabled { get; set; }
        private IMessenger Messenger { get; }

        public Slider Range { get; }

        private string _modifier;
        [OSC]
        public string Modifier
        {
            get => _modifier;
            set
            {
                SetAndNotify(ref _modifier, value);
                if (MessageEnabled)
                    Messenger.SendMessage(MessageAddress + nameof(Modifier), Modifier);
            }
        }
        #endregion

        #region COPY/PASTE
        public void Copy(RangeControlDTO rangecontroldto)
        {
            Range.Copy(rangecontroldto.Range);
            rangecontroldto.Modifier = Modifier;
        }

        public void Paste(RangeControlDTO rangecontroldto)
        {
            MessageEnabled = false;
            Range.Paste(rangecontroldto.Range);
            Modifier = rangecontroldto.Modifier;
            MessageEnabled = true;
        }
        #endregion
    }
}