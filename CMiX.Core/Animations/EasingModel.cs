// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
