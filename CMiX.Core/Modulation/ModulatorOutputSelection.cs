// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modulation.Modulators;

namespace CMiX.Core.Modulation
{
    public record ModulatorOutputSelection(IModulator Modulator, IModulatorOutput Output);
}
