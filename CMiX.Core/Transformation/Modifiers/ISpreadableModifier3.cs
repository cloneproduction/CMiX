// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;

namespace CMiX.Core.Transformation.Modifiers
{
    public interface ISpreadableModifier3 : IModifier
    {
        public ModulatableInteger3 Count { get; }
    }
}
