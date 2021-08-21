// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Network.Messages
{
    public class MessageSelectedMasterBeatChange
    {
        public MessageSelectedMasterBeatChange(MasterBeat oldMasterBeat, MasterBeat newMasterBeat)
        {
            OldMasterBeat = oldMasterBeat;
            NewMasterBeat = newMasterBeat;
        }

        public MasterBeat OldMasterBeat { get; set; }
        public MasterBeat NewMasterBeat { get; set; }
    }
}
