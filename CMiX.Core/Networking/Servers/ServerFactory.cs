// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Ceras;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Networking.Servers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Network
{
    public class ServerFactory
    {
        public ServerFactory(ServerRepository serverRepository, CerasSerializer cerasSerializer)
        {
            Serializer = cerasSerializer;
            ServerRepository = serverRepository;
        }

        ServerRepository ServerRepository { get; set; }
        CerasSerializer Serializer { get; set; }

        int ID = 0;

        public Server CreateServer(Settings settings)
        {
            var server = new Server(settings, Serializer);
            ID++;
            ServerRepository.AddServer(server);
            return server;
        }
    }
}
