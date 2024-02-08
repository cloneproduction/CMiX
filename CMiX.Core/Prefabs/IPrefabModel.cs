// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public interface IPrefabModel : IControlModel
    {
        PrefabServiceModel PrefabService { get; set; }
        GenericValueModel<bool> Visibility { get; set; }
        GenericValueModel<bool> IsSelected { get; set; }
        GenericValueModel<bool> IsRenaming { get; set; }
        GenericValueModel<string> Name { get; set; }
    }
}
