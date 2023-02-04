// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Scale : ObservableObject, ITransformModifier
    {
        public Scale(ScaleModel scaleModel)
        {
            this.ID = scaleModel.ID;

            Uniform = new FloatValue(scaleModel.Uniform);
            XYZ = new Vector3(scaleModel.XYZ);
            Visible = new BooleanValue(scaleModel.Visible);

            Mode = new GenericValue<ModifierMode>(scaleModel.Mode);
            IsExpanded = true;
        }


        public Guid ID { get; set; }

        public FloatValue Uniform { get; set; }
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void Dispose()
        {
            
        }
    }
}
