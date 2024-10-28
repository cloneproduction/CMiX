// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public record LayerSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<BlendModeEnum> BlendMode { get; set; } = new(BlendModeEnum.Normal);
        public GenericValueModel<float> Opacity { get; set; } = new(1.0f);
        public GenericValueModel<string> BackgroundColor { get; set; } = new("#FF000000");
    }
}
