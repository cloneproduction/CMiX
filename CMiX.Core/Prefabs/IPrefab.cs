// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public interface IPrefab : IControl
    {

        PrefabService PrefabService { get; set; }
        GenericValue<bool> Visibility { get; set; }
        GenericValue<bool> IsSelected { get; set; }
        GenericValue<bool> IsRenaming { get; set; }
        GenericValue<string> Name { get; set; }
    }
}
