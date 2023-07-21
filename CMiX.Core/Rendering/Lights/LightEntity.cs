// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightEntity : ObservableObject, IPrefab
    {
        public LightEntity(CompositionService compositionService)
        {
            CompositionService = compositionService;

            IsRenaming = new BooleanValue();
            IsSelected = new BooleanValue();
            LightColor = new ColorSelector();
            Position = new Vector3();
            Target = new Vector3();
            Radius = new FloatValue();
            Angle = new FloatValue();
            Softness = new FloatValue();
            Intensity = new FloatValue();
            LightTypeSelector = new GenericValue<LightType>();
            Visibility = new BooleanValue();
            IsSelected = new BooleanValue();
            Name = new StringValue();

            ModifierManager = new ModifierManager(new EntityModifierFactory());
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModifierManager ModifierManager { get; set; }
        public CompositionService CompositionService { get; set; }
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
    }
}
