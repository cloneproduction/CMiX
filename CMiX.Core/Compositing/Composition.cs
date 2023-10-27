// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public class Composition : ObservableObject, IPrefab, IModifiable
    {
        public Composition(PrefabService prefabService, 
                           MasterBeat masterBeat, 
                           PrefabManager prefabManager, 
                           ModifierManager modifierManager, 
                           OutputSettings outputSettings)
        {
            Name = prefabService.Name;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;
            Visibility = prefabService.Visibility;
            MasterBeat = masterBeat;
            OutputSettings = outputSettings;
            LayerManager = prefabManager;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue Visibility { get; set; }
        public PrefabManager LayerManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public MasterBeat MasterBeat { get; set; }
    }
}
