// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public record GenericValueModel<T> : IControlModel
    {
        public GenericValueModel() { }
        public GenericValueModel(T value) => Value = value;
        public Guid ID { get; init; } = Guid.NewGuid();
        public T Value { get; init; }
    }
}
