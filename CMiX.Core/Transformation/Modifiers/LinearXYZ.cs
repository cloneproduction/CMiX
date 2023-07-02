// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class LinearXYZ : ObservableObject, IModifier, ISpreadable
    {
        public LinearXYZ(LinearXYZModel linearXYZModel)
        {
            ID = linearXYZModel.ID;
            
            Visible = new BooleanValue(linearXYZModel.Visible);
            ModifierModeSelector = new ModifierModeSelector(linearXYZModel.ModifierModeSelector);
            Width = new FloatValue(linearXYZModel.Width);
            Phase = new FloatValue(linearXYZModel.Phase);
            TransformTypeSelector = new GenericValue<TransformType>(linearXYZModel.TransformTypeSelector);
            DirectionXYZ = new DirectionXYZ(linearXYZModel.DirectionXYZ);

            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public FloatValue Width { get; set; }
        public FloatValue Phase { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }


        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
