// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Text.Modifiers
{
    public class CharWriterModel : IPrefabModel
    {
        public CharWriterModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifierManager = new PrefabManagerModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }


    }
}
