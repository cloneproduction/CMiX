// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Managers
{
    public record PrefabSelectorModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public Guid SelectedItemID { get; init; } = Guid.Empty;
        public IControlModel SelectedItemModel { get; init; } = null;
    }
}
