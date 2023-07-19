// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class LinearXYZ : ObservableObject, IModifier, ISpreadable
    {
        public LinearXYZ()
        {
            Visible = new BooleanValue(true);
            ModifierModeSelector = new ModifierModeSelector();
            TransformTypeSelector = new GenericValue<TransformType>();
            Width = new FloatValue(0);
            Phase = new FloatValue(0);
            DirectionXYZ = new DirectionXYZ();
        }


        public Guid ID { get; set; } = Guid.NewGuid();

        public BooleanValue Visible { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public FloatValue Width { get; set; }
        public FloatValue Phase { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;
    }
}
