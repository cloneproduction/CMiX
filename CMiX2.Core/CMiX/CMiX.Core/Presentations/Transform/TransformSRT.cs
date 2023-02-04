// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public partial class TransformSRT : ObservableObject, IControl, IModifier
    {
        public TransformSRT(TransformSRTModel transformModel)
        {
            isExpanded = true;
            ID = transformModel.ID;

            Visible = new BooleanValue(transformModel.Visible);
            Uniform = new FloatValue(transformModel.Uniform);
            Translate = new Translate(transformModel.Translate);
            Scale = new Scale(transformModel.Scale);
            Rotation = new Rotation(transformModel.Rotation);
            Mode = new GenericValue<ModifierMode>(transformModel.Mode);
        }


        public Guid ID { get; set; }
        public FloatValue Uniform { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }


        [ObservableProperty]
        private bool isExpanded;


        public void Dispose()
        {

        }
    }
}
