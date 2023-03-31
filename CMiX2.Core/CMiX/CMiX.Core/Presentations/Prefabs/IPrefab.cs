// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Presentations.Prefabs
{
    public interface IPrefab : IControl
    {
        bool IsSelected { get; set; }
        bool IsRenaming { get; set; }
        string Name { get; set; }
    }
}
