// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Networking.Servers
{
    public record ServerModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<string> IP { get; set; } = new("127.0.0.1");
        public GenericValueModel<int> Port { get; set; } = new(8080);
    }
}
