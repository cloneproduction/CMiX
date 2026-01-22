// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public record EmptyPrefabModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<bool> Visibility { get; set; } = new(false);
        public GenericValueModel<bool> IsSelected { get; set; } = new(false);
        public GenericValueModel<bool> IsRenaming { get; set; } = new(false);
        public GenericValueModel<string> Name { get; set; } = new("Empty");
    }
}
