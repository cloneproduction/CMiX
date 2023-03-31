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
            IsExpanded = true;

            CompositionService = compositionService;

            Visible = new BooleanValue(triColorModel.Visible);
            Control = new FloatValue(triColorModel.Control);

            ColorA = new ColorSelector(triColorModel.ColorA);
            ColorB = new ColorSelector(triColorModel.ColorB);
            ColorC = new ColorSelector(triColorModel.ColorC);

            Smooth = new FloatValue(triColorModel.Smooth);
            Center = new FloatValue(triColorModel.Center);

            SingleChannel = new BooleanValue(triColorModel.SingleChannel);
            ClampColor = new BooleanValue(triColorModel.ClampColor);
        }

        public CompositionService CompositionService { get; set; }


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
