// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    internal class MessageButtonClicked : IMessage
    {
        public MessageButtonClicked()
        {

        }

        public MessageButtonClicked(IControl control)
        {
            ID = control.ID;
        }

        public Guid ID { get; set; }
    }
}
