// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public class Composition : ObservableObject, IPrefab, IModifiable
    {
        public Composition(CompositionService compositionService)
        {
            CompositionService = compositionService;
            MasterBeat = new MasterBeat();
            CompositionService.MasterBeat = MasterBeat;
            LayerManager = new DraggablePrefabManager<Layer>(compositionService);

            Name = new StringValue();
            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();
            OutputSettings = new OutputSettings();

            ModifierManager = new ModifierManager(new TextureFilterFactory());
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public PrefabManager<Layer> LayerManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public CompositionService CompositionService { get; set; }
    }
}
