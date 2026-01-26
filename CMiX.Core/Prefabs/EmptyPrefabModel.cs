// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public record EmptyPrefabModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public GenericValueModel<bool> Visibility { get; init; } = new(false);
        public GenericValueModel<bool> IsSelected { get; init; } = new(false);
        public GenericValueModel<bool> IsRenaming { get; init; } = new(false);
        public GenericValueModel<string> Name { get; init; } = new("Empty");
    }
}
