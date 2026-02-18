// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Networking
{
    public class ControlMessenger
    {
        public ControlMessenger(ControlRepository controlRepository)
        {
            Servers = controlRepository.Servers;
        }

        ObservableCollection<Server> Servers { get; }

        public void SendMessage(IMessage message)
        {
            Servers.ToList().ForEach(server => server.SendMessage(message));
        }
    }
}
