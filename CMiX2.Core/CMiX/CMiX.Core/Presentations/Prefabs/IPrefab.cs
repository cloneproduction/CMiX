// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Prefabs
{
    public interface IPrefab : IControl
    {
        BooleanValue IsSelected { get; set; }
        BooleanValue IsRenaming { get; set; }
        StringValue Name { get; set; }
    }
}
