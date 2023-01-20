// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Presentation.ViewModels.Network
{
    public interface IMessageService
    {
        void SendMessage(IMessage message);
        ObservableCollection<Server> Servers { get; set; }
    }
}
