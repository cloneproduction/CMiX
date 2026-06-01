// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class CircularSpread : ObservableObject, ISpreadableModifier
    {
        public CircularSpread(PrefabService prefabService, 
                              ModifierModeSelector modifierModeSelector, 
                              Vector2 width, 
                              GenericValue<float> phase,
                              GenericValue<float> factor)
        {
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            Factor = factor;
            Width = width;
            Phase = phase;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector2 Width { get; set; }
        public GenericValue<float> Phase { get; set; }
        public GenericValue<float> Factor { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new CircularSpreadModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
            Width = (Vector2Model)Width.ToModel(),
            Phase = (GenericValueModel<float>)Phase.ToModel(),
            Factor = (GenericValueModel<float>)Factor.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CircularSpreadModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            Width.FromModel(m.Width);
            Phase.FromModel(m.Phase);
            Factor.FromModel(m.Factor);
        }
    }
}
