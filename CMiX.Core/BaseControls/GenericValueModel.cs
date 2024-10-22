// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public record GenericValueModel<T> : IControlModel
    {
        public GenericValueModel()
        {

        }

        public GenericValueModel(T value)
        {
            Value = value;
        }

        public Guid ID { get; init; } = Guid.NewGuid();
        public T Value { get; init; }
    }
}
