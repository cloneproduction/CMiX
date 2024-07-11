// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class HSCBModel : IControlModel
    {
        public HSCBModel()
        {
            PrefabService = new PrefabServiceModel();
            Hue = new GenericValueModel<float>(0.0f);
            Saturation = new GenericValueModel<float>(1.0f);
            Contrast = new GenericValueModel<float>(0.0f);
            Brightness = new GenericValueModel<float>(0.0f);
            Control = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Hue { get; set; }
        public GenericValueModel<float> Saturation { get; set; }
        public GenericValueModel<float> Contrast { get; set; }
        public GenericValueModel<float> Brightness { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
