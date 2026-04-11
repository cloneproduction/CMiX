// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;
using VL.Lib.Mathematics;
namespace CMiX.Core.Animations
{
    public partial class Easing : ObservableRecipient, IControl
    {
        public Easing(GenericValue<bool> isEnabled, 
                      GenericValue<TweenerTransition> tweenerTransition, 
                      GenericValue<TweenerMode> tweenerMode)
        {
            IsEnabled = isEnabled;
            Transition = tweenerTransition;
            Mode = tweenerMode;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<TweenerTransition> Transition { get; set; }
        public GenericValue<TweenerMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new EasingModel
        {
            ID = ID,
            IsEnabled = (GenericValueModel<bool>)IsEnabled.ToModel(),
            Transition = (GenericValueModel<TweenerTransition>)Transition.ToModel(),
            Mode = (GenericValueModel<TweenerMode>)Mode.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (EasingModel)model;
            ID = m.ID;
            IsEnabled.FromModel(m.IsEnabled);
            Transition.FromModel(m.Transition);
            Mode.FromModel(m.Mode);
        }
    }
}
