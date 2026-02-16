// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Text.Modifiers
{
    public partial class Split : ObservableObject, IPrefab
    {
        public Split(PrefabService prefabService,
                     GenericValue<string> separator,
                     GenericValue<SplitType> type)
        {
            PrefabService = prefabService;
            Separator = separator;
            Type = type;
        }

        public IControlModel ToModel() => this.ToModel();
        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }

        public GenericValue<string> Separator { get; set; }
        public GenericValue<SplitType> Type { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
