// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Ceras;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Network
{
    public class ServerFactory
    {
        public ServerFactory(CerasSerializer cerasSerializer)
        {
            Serializer = cerasSerializer;
        }

        CerasSerializer Serializer { get; set; }

        int ID = 0;

        public Server CreateServer(Settings settings)
        {
            var server = new Server(settings, Serializer);
            ID++;
            return server;
        }
    }
}
