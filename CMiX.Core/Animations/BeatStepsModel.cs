// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    public record BeatStepsModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public List<GenericValueModel<bool>> Steps { get; init; } = new();
    }
}
