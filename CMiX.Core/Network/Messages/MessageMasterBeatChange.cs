// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Network.Messages
{
    public class MessageMasterBeatChange : IMessage
    {
        public MessageMasterBeatChange()
        {

        }

        public MessageMasterBeatChange(MasterBeat masterBeat)
        {
            MasterBeat = masterBeat;
        }

        public MessageMasterBeatChange(MasterBeat masterBeat, Guid componentId) : this(masterBeat)
        {
            ID = componentId;
        }

        public MasterBeat MasterBeat { get; set; }
        public Guid ID { get; set; }
    }
}
