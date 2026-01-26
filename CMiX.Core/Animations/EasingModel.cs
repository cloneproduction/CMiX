// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    public record EasingModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsEnabled { get; set; } = new(false);
        public GenericValueModel<EasingFunction> Function { get; set; } = new(EasingFunction.Linear);
        public GenericValueModel<EasingMode> Mode { get; set; } = new(EasingMode.In);
    }
}
