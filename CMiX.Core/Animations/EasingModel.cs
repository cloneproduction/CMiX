// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using VL.Lib.Mathematics;

namespace CMiX.Core.Animations
{
    public record EasingModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsEnabled { get; set; } = new(false);
        public GenericValueModel<TweenerTransition> Transition { get; set; } = new(TweenerTransition.Linear);
        public GenericValueModel<TweenerMode> Mode { get; set; } = new(TweenerMode.In);
    }
}
