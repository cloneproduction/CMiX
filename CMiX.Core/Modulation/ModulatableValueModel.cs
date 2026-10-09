// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation
{
    public record ModulatableValueModel<T> : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public string Label { get; init; } = string.Empty;
        public GenericValueModel<T> Value { get; init; } = new(default);
        public GenericValueModel<Guid?> ModulatorID { get; init; } = new(null);
        public GenericValueModel<string> BoundOutputName { get; init; } = new(null);

        // The default of a value: its label and its start value.
        public static ModulatableValueModel<T> Of(string label, T value) =>
            new() { Label = label, Value = new GenericValueModel<T>(value) };
    }
}
