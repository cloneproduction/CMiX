// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageTextInputFocusState : IMessage
    {
        public MessageTextInputFocusState(IControl control, bool focusState)
        {
            ID = control.ID;
            FocusState = focusState;
        }

        public Guid ID { get; set; }
        public bool FocusState { get; set; }
    }
}
