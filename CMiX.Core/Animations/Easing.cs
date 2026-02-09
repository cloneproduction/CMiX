// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Animations
{
    public partial class Easing : ObservableRecipient, IControl
    {
        public Easing(GenericValue<bool> isEnabled, 
                      GenericValue<EasingFunction> easingFunction, 
                      GenericValue<EasingMode> easingMode)
        {
            IsEnabled = isEnabled;
            Function = easingFunction;
            Mode = easingMode;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<EasingFunction> Function { get; set; }
        public GenericValue<EasingMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
