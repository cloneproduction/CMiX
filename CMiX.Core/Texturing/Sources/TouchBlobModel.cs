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
            Resolution = new Integer2Model(1920, 1080);
            Size = new GenericValueModel<float>(0.2f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2Model Resolution { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Size { get; set; }
        public PrefabManagerModel FilterManager { get; set; }
    }
}
