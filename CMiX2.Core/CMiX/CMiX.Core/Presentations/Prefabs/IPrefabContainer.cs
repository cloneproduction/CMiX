// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Presentation.Prefabs
{
    public interface IPrefabContainer : IPrefab
    {
        IPrefab Prefab { get; set; }
    }
}
