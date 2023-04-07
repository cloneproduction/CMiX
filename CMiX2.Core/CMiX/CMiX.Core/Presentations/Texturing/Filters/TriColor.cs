// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class TriColor : ObservableObject, ITextureFilter
    {
        public TriColor(TriColorModel triColorModel, CompositionService compositionService)
        {
            ID = triColorModel.ID;
            Name = triColorModel.Name;
            isExpanded = true;

            Visible = new BooleanValue(triColorModel.Visible, compositionService);
            Control = new FloatValue(triColorModel.Control, compositionService);

            ColorA = new ColorSelector(triColorModel.ColorA, compositionService);
            ColorB = new ColorSelector(triColorModel.ColorB, compositionService);
            ColorC = new ColorSelector(triColorModel.ColorC, compositionService);

            Smooth = new FloatValue(triColorModel.Smooth, compositionService);
            Center = new FloatValue(triColorModel.Center, compositionService);

            SingleChannel = new BooleanValue(triColorModel.SingleChannel, compositionService);
            ClampColor = new BooleanValue(triColorModel.ClampColor, compositionService);
        }

        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public FloatValue Control { get; set; }
        public FloatValue Smooth { get; set; }
        public FloatValue Center { get; set; }
        public ColorSelector ColorA { get; set; }
        public ColorSelector ColorB { get; set; }
        public ColorSelector ColorC { get; set; }
        public BooleanValue SingleChannel { get; set; }
        public BooleanValue ClampColor { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private bool enabled;

        public void Dispose()
        {

        }
    }
}
