// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public class Easing : ObservableRecipient, IControl
    {
        public Easing(EasingModel easingModel, CompositionService compositionService)
        {
            this.ID = easingModel.ID;
            IsEnabled = new BooleanValue(easingModel.IsEnabled, compositionService);
            Mode = new GenericValue<EasingMode>(easingModel.Mode, compositionService);
            Function = new GenericValue<EasingFunction>(easingModel.Function, compositionService);
        }

        public Guid ID { get; set; }
        public BooleanValue IsEnabled { get; set; }
        public GenericValue<EasingFunction> Function { get; set; }
        public GenericValue<EasingMode> Mode { get; set; }
    }
}
