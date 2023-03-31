// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Pixelate : ObservableObject, ITextureFilter
    {
        public Pixelate(PixelateModel pixelateModel)
        {
            ID = pixelateModel.ID;
            Visible = new BooleanValue(pixelateModel.Visible);
            Name = pixelateModel.Name;
            Control = new FloatValue(pixelateModel.Control);
            Factor = new Vector2(pixelateModel.Factor);

            IsExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }

        public Vector2 Factor { get; set; }
        public FloatValue Control { get; set; }

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
