// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public class TouchBlobModel : IControlModel, IPrefabModel
    {
        public TouchBlobModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            FilterManager = new PrefabManagerModel();
            Resolution = new Integer2Model(1024, 1024);
            Size = new GenericValueModel<float>(0.2f);
            Color = new GenericValueModel<string>("#FFFFFF");
            Background = new GenericValueModel<string>("#000000");
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2Model Resolution { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Size { get; set; }
        public PrefabManagerModel FilterManager { get; set; }
        public GenericValueModel<string> Color { get; set; }
        public GenericValueModel<string> Background { get; set; }
    }
}
