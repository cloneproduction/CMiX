// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class CMiXButton : ReceivableControl, IControl, IRecipient<IMessage>
    {
        public CMiXButton(IValueSync sync)
        {
            ID = Guid.NewGuid();
            _sync = sync;
            ClickCommand = new RelayCommand(OnClick);
            IsActive = true;
        }

        private readonly IValueSync _sync;

        public ICommand ClickCommand { get; }

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        public void OnClick()
        {
            Click?.Invoke(this, null);
            _sync.Send(this);
        }

        public Guid ID { get; set; }

        public void Receive(IMessage message)
        {
            if (message.ID != ID || message is not MessageValueChanged) return;
            Click?.Invoke(this, null);
        }

        public IControlModel ToModel() => new ButtonModel
        {
            ID = ID
        };

        public void FromModel(IControlModel model)
        {
            var m = (ButtonModel)model;
            ID = m.ID;
        }
    }
}
