// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Colors.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class ColorPalette : ObservableObject, IModifier, IDisposable
    {
        public ColorPalette(PrefabService prefabService,
                            PrefabManager colorManager,
                            PrefabManager modifierManager,
                            GenericValue<ResamplingMethod> resample)
        {
            PrefabService = prefabService;
            ColorManager = colorManager;
            ModifierManager = modifierManager;
            Resample = resample;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager ColorManager { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public GenericValue<ResamplingMethod> Resample { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new ColorPaletteModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            ColorManager = (PrefabManagerModel)ColorManager.ToModel(),
            Resample = (GenericValueModel<ResamplingMethod>)Resample.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ColorPaletteModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Resample.FromModel(m.Resample);

            LoadManager(ColorManager, m.ColorManager);
            LoadManager(ModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            ColorManager.ClearAll();
            ModifierManager.ClearAll();
        }
    }
}
