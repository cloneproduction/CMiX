// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightEntity : ObservableObject, IPrefab
    {
        public LightEntity(CompositionService compositionService)
        {
            IsRenaming = new BooleanValue(false);
            IsSelected = new BooleanValue(false);
            LightColor = new ColorSelector();
            Position = new Vector3(0.0f, 2.0f, 0.0f);
            Target = new Vector3(0.001f, 0.0f, 0.0f);
            Radius = new FloatValue(5.0f);
            Angle = new FloatValue(0.25f);
            Softness = new FloatValue(0.01f);
            Intensity = new FloatValue(1.0f);
            LightTypeSelector = new GenericValue<LightType>(LightType.AmbientLight);
            Visibility = new BooleanValue();
            Name = new StringValue("Light");

            ModifierManager = compositionService.GetModifierManager<LightEntity>();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModifierManager ModifierManager { get; set; }
        public GenericValue<LightType> LightTypeSelector { get; set; }
        public ColorSelector LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Angle { get; set; }
        public FloatValue Softness { get; set; }
        public FloatValue Intensity { get; set; }
        public BooleanValue Visibility { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }


        [ObservableProperty]
        private bool settingsIsExpanded;

        [ObservableProperty]
        private bool modifierIsExpanded;
    }
}
