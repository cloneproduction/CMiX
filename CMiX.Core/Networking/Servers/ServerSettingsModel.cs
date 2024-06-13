// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.


using CMiX.Core.BaseControls;

namespace CMiX.Core.Networking
{
    public class ServerSettingsModel : IControlModel
    {
        public ServerSettingsModel()
        {
            ID = Guid.NewGuid();
            IP = new GenericValueModel<string>();
            Port = new GenericValueModel<int>();
            Message = new GenericValueModel<string>();
        }

        public Guid ID { get; set; }
        public GenericValueModel<string> IP { get; set; }
        public GenericValueModel<int> Port { get; set; }
        public GenericValueModel<string> Message { get; set; }
    }
}
