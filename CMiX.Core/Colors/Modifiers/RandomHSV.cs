// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Colors.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class RandomHSV : ObservableObject, IBeatModifiable, IPrefab, ISpreadableModifier
    {
        public RandomHSV(PrefabManager beatModifierManager,
                         PrefabService prefabService, 
                         ModifierModeSelector modifierModeSelector,
                         GenericValue<ColorMode> colorMode,
                         GenericValue<float> hue, 
                         GenericValue<float> saturation, 
                         GenericValue<float> value, 
                         GenericValue<float> alpha)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            ColorMode = colorMode;
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<ColorMode> ColorMode { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Value { get; set; }
        public GenericValue<float> Alpha { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new RandomHSVModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            ColorMode = (GenericValueModel<ColorMode>)ColorMode.ToModel(),
            Hue = (GenericValueModel<float>)Hue.ToModel(),
            Saturation = (GenericValueModel<float>)Saturation.ToModel(),
            Value = (GenericValueModel<float>)Value.ToModel(),
            Alpha = (GenericValueModel<float>)Alpha.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomHSVModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            ColorMode.FromModel(m.ColorMode);
            Hue.FromModel(m.Hue);
            Saturation.FromModel(m.Saturation);
            Value.FromModel(m.Value);
            Alpha.FromModel(m.Alpha);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
    }
}
