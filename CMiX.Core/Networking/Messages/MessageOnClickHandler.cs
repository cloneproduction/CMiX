// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Networking.Messages
{
    public class MessageOnClickHandler : IMessageHandler
    {
        public MessageOnClickHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if(control is Button btn)
            {
                if (message is MessageOnClick messageOnClick)
                {
                    btn.OnClick();
                    return true;
                }
            }

            return false;
        }
    }
}
