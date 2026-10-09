// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public interface IModulator : IPrefab, IHasCompositionID
    {
        bool IsHovered { get; set; }

        bool IsExpanded { get; set; }

        IReadOnlyList<IModulatorOutput> Outputs { get; }
    }
}
