// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
    public partial class ColorPaletteModifier : ObservableObject, IModifier, IDisposable
    {
        public ColorPaletteModifier(PrefabService prefabService,
                                    PrefabManager colorManager,
                                    GenericValue<ResamplingMethod> resample)
        {
            PrefabService = prefabService;
            ColorManager = colorManager;
            Resample = resample;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager ColorManager { get; set; }
        public GenericValue<ResamplingMethod> Resample { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new ColorPaletteModifierModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            ColorManager = (PrefabManagerModel)ColorManager.ToModel(),
            Resample = (GenericValueModel<ResamplingMethod>)Resample.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ColorPaletteModifierModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Resample.FromModel(m.Resample);

            LoadManager(ColorManager, m.ColorManager);
        }
        public void Dispose() => DisposeAll(ColorManager);
    }
}
