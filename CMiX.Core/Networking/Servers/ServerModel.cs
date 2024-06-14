// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking.Servers
{
    public class ServerModel : IControlModel
    {
        public ServerModel()
        {
            ServerSettings = new ServerSettingsModel();
        }
        public Guid ID { get; set; }
        public ServerSettingsModel  ServerSettings { get; set; }
    }
}
