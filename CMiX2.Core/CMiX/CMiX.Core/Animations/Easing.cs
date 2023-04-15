// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Animations
{
    public class Easing : ObservableRecipient, IControl
    {
        public Easing(EasingModel easingModel)
        {
            this.ID = easingModel.ID;
            IsEnabled = new BooleanValue(easingModel.IsEnabled);
            Mode = new GenericValue<EasingMode>(easingModel.Mode);
            Function = new GenericValue<EasingFunction>(easingModel.Function);
        }

        public Guid ID { get; set; }
        public BooleanValue IsEnabled { get; set; }
        public GenericValue<EasingFunction> Function { get; set; }
        public GenericValue<EasingMode> Mode { get; set; }
    }
}
