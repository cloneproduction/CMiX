// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blend : ObservableRecipient, IControl
    {
        public Blend(GenericValue<bool> isEnabled,
                     GenericValue<BlendModeEnum> blendMode,
                     GenericValue<float> opacity)
        {
            IsEnabled = isEnabled;
            BlendMode = blendMode;
            Opacity = opacity;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public GenericValue<float> Opacity { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new BlendModel
        {
            ID = ID,
            IsEnabled = (GenericValueModel<bool>)IsEnabled.ToModel(),
            BlendMode = (GenericValueModel<BlendModeEnum>)BlendMode.ToModel(),
            Opacity = (GenericValueModel<float>)Opacity.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BlendModel)model;
            ID = m.ID;
            IsEnabled.FromModel(m.IsEnabled);
            BlendMode.FromModel(m.BlendMode);
            Opacity.FromModel(m.Opacity);
        }
    }
}
