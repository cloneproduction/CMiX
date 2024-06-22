// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Networking.Servers
{
    public class ServerModel : IControlModel
    {
        public ServerModel()
        {
            IP = new GenericValueModel<string>("127.0.0.1");
            Port = new GenericValueModel<int>();
        }
        public Guid ID { get; set; }

        public GenericValueModel<string> IP { get; set; }
        public GenericValueModel<int> Port { get; set; }
    }
}
