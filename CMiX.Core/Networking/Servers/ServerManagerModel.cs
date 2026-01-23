// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Networking.Servers
{
    public record ServerManagerModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public ManagerDataModel ManagerData { get; set; } = new();
    }
}
