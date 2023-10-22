// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public interface IPrefabModel : IControlModel
    {
        BooleanValueModel Visibility { get; set; }
        BooleanValueModel IsSelected { get; set; }
        BooleanValueModel IsRenaming { get; set; }
        StringValueModel Name { get; set; }
    }
}
