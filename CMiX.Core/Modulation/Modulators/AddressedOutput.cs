// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Modulation.Modulators
{
    // A float output with the address it reads from.
    public record AddressedOutput(string Name, string Address) : ModulatorOutput<float>(Name);
}
