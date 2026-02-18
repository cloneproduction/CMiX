// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Prefabs.Managers
{
    public record ManagerDataModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Collection<IControlModel> Items { get; set; } = new();
        public int SelectedIndex { get; set; }
    }
}
