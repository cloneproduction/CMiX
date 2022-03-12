// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Network.Messages
{
    public class MessageSelectMasterBeat : IMessage
    {
        public MessageSelectMasterBeat(Guid componentID, MasterBeat masterBeat)
        {
            ID = componentID;
            MasterBeatModel = masterBeat.GetModel();
        }

        public IModel MasterBeatModel { get; set; }
        public Guid ID { get; set; }

    }
}
