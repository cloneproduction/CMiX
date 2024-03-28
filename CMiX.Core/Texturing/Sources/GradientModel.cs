// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Sources
{
    public class GradientModel : IControlModel, IPrefabModel
    {
        public GradientModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Resolution = new Integer2Model(512, 512);
            From = new GenericValueModel<string>("#FFFFFFFF");
            To = new GenericValueModel<string>("#FF000000");
            Gamma = new GenericValueModel<float>(2.2f);
            Horizontal = new GenericValueModel<bool>(false);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public Integer2Model Resolution { get; set; }
        public GenericValueModel<float> Gamma { get; set; }
        public GenericValueModel<string> From { get; set; }
        public GenericValueModel<string> To { get; set; }
        public GenericValueModel<bool> Horizontal { get; set; }
        public GenericValueModel<string> BackgroundColor { get; set; }
    }
}
