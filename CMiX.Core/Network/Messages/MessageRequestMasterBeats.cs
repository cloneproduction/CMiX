// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CMiX.Core.Network.Messages
{
    public class MessageRequestMasterBeats : RequestMessage<ObservableCollection<MasterBeat>>
    {
    }
}
