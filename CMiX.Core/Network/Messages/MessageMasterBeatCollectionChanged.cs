// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Network.Messages
{
    public class MessageMasterBeatCollectionChanged : IMessage
    {
        public MessageMasterBeatCollectionChanged(ObservableCollection<MasterBeat> masterBeats)
        {
            MasterBeats = masterBeats;
        }

        public ObservableCollection<MasterBeat> MasterBeats { get; set; }
        public Guid ID { get; set; }
    }
}
