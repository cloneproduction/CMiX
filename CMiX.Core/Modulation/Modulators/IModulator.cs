// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public interface IModulator : IPrefab
    {
        bool IsHovered { get; set; }

        bool IsExpanded { get; set; }

        IReadOnlyList<IModulatorOutput> Outputs { get; }
    }
}
