// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

            Visible = new ToggleButton(triColorModel.Visible);
            Control = new Slider(nameof(Control), triColorModel.Control);

            ColorA = new ColorSelector(triColorModel.ColorA);
            ColorB = new ColorSelector(triColorModel.ColorB);
            ColorC = new ColorSelector(triColorModel.ColorC);

            Smooth = new Slider(nameof(Smooth), triColorModel.Smooth);
            Center = new Slider(nameof(Center), triColorModel.Center);

            SingleChannel = new ToggleButton(triColorModel.SingleChannel);
            ClampColor = new ToggleButton(triColorModel.ClampColor);

            OpenColorASelectorCommand = new RelayCommand(OpenColorASelector);
            OpenColorBSelectorCommand = new RelayCommand(OpenColorBSelector);
            OpenColorCSelectorCommand = new RelayCommand(OpenColorCSelector);
        }


        public ICommand OpenColorASelectorCommand { get; set; }
        public ICommand OpenColorBSelectorCommand { get; set; }
        public ICommand OpenColorCSelectorCommand { get; set; }

        public CompositionService CompositionService { get; set; }


        public void OpenColorASelector()
        {
            //CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.ColorA);
        }

        public void OpenColorBSelector()
        {
            //CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.ColorB);
        }

        public void OpenColorCSelector()
        {
            //CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.ColorC);
        }


        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public ToggleButton Visible { get; set; }

        public Slider Control { get; set; }
        public Slider Smooth { get; set; }
        public Slider Center { get; set; }

        public ColorSelector ColorA { get; set; }
        public ColorSelector ColorB { get; set; }
        public ColorSelector ColorC { get; set; }

        public ToggleButton SingleChannel { get; set; }
        public ToggleButton ClampColor { get; set; }


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

            triColorModel.Visible = (ToggleButtonModel)Visible.GetModel();
            triColorModel.Control = (SliderModel)Control.GetModel();
            triColorModel.ColorA = (ColorSelectorModel)ColorA.GetModel();
            triColorModel.ColorB = (ColorSelectorModel)ColorB.GetModel();
            triColorModel.ColorC = (ColorSelectorModel)ColorC.GetModel();
            triColorModel.Smooth = (SliderModel)Smooth.GetModel();
            triColorModel.Center = (SliderModel)Center.GetModel();
            triColorModel.SingleChannel = (ToggleButtonModel)SingleChannel.GetModel();
            triColorModel.ClampColor = (ToggleButtonModel)ClampColor.GetModel();

            return triColorModel;
        }

        public void Dispose()
        {

        }
    }
}
