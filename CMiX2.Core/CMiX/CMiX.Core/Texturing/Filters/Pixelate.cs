// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : ObservableObject, ITextureFilter
    {
        public Pixelate(PixelateModel pixelateModel)
        {
            ID = pixelateModel.ID;
            Name = pixelateModel.Name;
            Visible = new BooleanValue(pixelateModel.Visible);
            Control = new FloatValue(pixelateModel.Control);
            Factor = new Vector2(pixelateModel.Factor);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public Vector2 Factor { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
