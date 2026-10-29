// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Modulation.Modulators
{
    // An output with the address it reads from.
    public interface IAddressedOutput
    {
        string Address { get; }
    }

    public record AddressedOutput<T>(string Name, string Address) : ModulatorOutput<T>(Name), IAddressedOutput;
}
