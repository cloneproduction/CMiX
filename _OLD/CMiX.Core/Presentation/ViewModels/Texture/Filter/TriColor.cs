// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TriColor : ObservableObject, ITextureFilter
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


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }


        public void SetViewModel(IModel model)
        {
            TriColorModel triColorModel = model as TriColorModel;
            ID = triColorModel.ID;
            Name = triColorModel.Name;

            Visible.SetViewModel(triColorModel.Visible);
            Control.SetViewModel(triColorModel.Control);
            ColorA.SetViewModel(triColorModel.ColorA);
            ColorB.SetViewModel(triColorModel.ColorB);
            ColorC.SetViewModel(triColorModel.ColorC);
            Smooth.SetViewModel(triColorModel.Smooth);
            Center.SetViewModel(triColorModel.Center);
            SingleChannel.SetViewModel(triColorModel.SingleChannel);
            ClampColor.SetViewModel(triColorModel.ClampColor);

        }

        public IModel GetModel()
        {
            TriColorModel triColorModel = new TriColorModel();
            triColorModel.ID = ID;
            triColorModel.Name = Name;

            triColorModel.Visible = (BooleanValueModel)Visible.GetModel();
            triColorModel.Control = (FloatValueModel)Control.GetModel();
            triColorModel.ColorA = (ColorSelectorModel)ColorA.GetModel();
            triColorModel.ColorB = (ColorSelectorModel)ColorB.GetModel();
            triColorModel.ColorC = (ColorSelectorModel)ColorC.GetModel();
            triColorModel.Smooth = (FloatValueModel)Smooth.GetModel();
            triColorModel.Center = (FloatValueModel)Center.GetModel();
            triColorModel.SingleChannel = (BooleanValueModel)SingleChannel.GetModel();
            triColorModel.ClampColor = (BooleanValueModel)ClampColor.GetModel();

            return triColorModel;
        }

        public void Dispose()
        {

        }
    }
}
