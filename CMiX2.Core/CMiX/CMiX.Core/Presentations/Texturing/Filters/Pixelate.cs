// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Pixelate : ObservableObject, ITextureFilter
    {
        public Pixelate(PixelateModel pixelateModel, CompositionService compositionService)
        {
            ID = pixelateModel.ID;
            Name = pixelateModel.Name;
            Visible = new BooleanValue(pixelateModel.Visible, compositionService);
            Control = new FloatValue(pixelateModel.Control, compositionService);
            Factor = new Vector2(pixelateModel.Factor, compositionService);
            isExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }

        public Vector2 Factor { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
