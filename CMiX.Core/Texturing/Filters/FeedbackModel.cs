// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class FeedbackModel : IPrefabModel
    {
        public FeedbackModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Factor = new GenericValueModel<float>(0.9f);
        }

      
        public Guid ID { get; set; }
        public GenericValueModel<float> Factor { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
