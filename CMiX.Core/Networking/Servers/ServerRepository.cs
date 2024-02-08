// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Networking.Messenger;

namespace CMiX.Core.Networking.Servers
{
    public class ServerRepository
    {
        public ServerRepository()
        {
            Servers = new ObservableCollection<Server>();
        }

        public ObservableCollection<Server> Servers { get; }

        public void AddServer(Server server)
        {
            Servers.Add(server);
        }

        public List<Server> GetServers()
        {
            return Servers.ToList();
        }
    }
}
