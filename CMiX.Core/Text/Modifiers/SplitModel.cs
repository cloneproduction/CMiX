// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Text.Modifiers
{
    public class SplitModel : IPrefabModel
    {
        public SplitModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Separator = new GenericValueModel<string>(" ");
            Type = new GenericValueModel<SplitType>(SplitType.Character);
        }

        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<string> Separator { get; set; }
        public GenericValueModel<SplitType> Type { get; set; }
        public Guid ID { get; set; }
    }
}
