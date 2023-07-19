// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Animations
{
    public partial class Easing : ObservableRecipient, IControl
    {
        public Easing()
        {
            IsEnabled = new BooleanValue(false);
            Function = new GenericValue<EasingFunction>(EasingFunction.Linear);
            Mode = new GenericValue<EasingMode>(EasingMode.In);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue IsEnabled { get; set; }
        public GenericValue<EasingFunction> Function { get; set; }
        public GenericValue<EasingMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
