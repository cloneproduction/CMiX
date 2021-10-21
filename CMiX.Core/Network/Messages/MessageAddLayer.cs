// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddLayer : IComponentMessage
    {
        public MessageAddLayer()
        {

        }

        public Guid ID { get; set; }
    }
}
