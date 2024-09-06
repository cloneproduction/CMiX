// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class LayerSettingsModel : IControlModel
    {
        public LayerSettingsModel() 
        {
            Opacity = new GenericValueModel<float>(1.0f);
            BackgroundColor = new GenericValueModel<string>("#FF000000");
            BlendMode = new GenericValueModel<BlendModeEnum>(BlendModeEnum.Normal);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<BlendModeEnum> BlendMode { get; set; }
        public GenericValueModel<float> Opacity { get; set; }
        public GenericValueModel<string> BackgroundColor { get; set; }
    }
}
