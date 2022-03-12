// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Network.Messages
{
    public class MessageSelectedMasterBeatChange : IMessage
    {
        public MessageSelectedMasterBeatChange()
        {

        }

        public MessageSelectedMasterBeatChange(Guid componentID, MasterBeat masterBeat)
        {
            ID = componentID;
            MasterBeatID = masterBeat.ID;
            MasterBeatModel = masterBeat.GetModel();
        }

        public IModel MasterBeatModel { get; set; }
        public Guid ID { get; set; }
        public Guid MasterBeatID { get; set; }
    }
}
