// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient(GradientModel gradientModel, CompositionService compositionService)
        {
            ID = gradientModel.ID;
            CompositionService = compositionService;
            ResolutionX = new Counter(gradientModel.ResolutionX);
            ResolutionY = new Counter(gradientModel.ResolutionY);
            Transform2D = new Transform2D(gradientModel.Transform2D, compositionService);
            From = new ColorSelector(gradientModel.From);
            To = new ColorSelector(gradientModel.To);
            Gamma = new Slider(nameof(Gamma), gradientModel.Gamma);
            Horizontal = new ToggleButton(gradientModel.Horizontal);

            OpenColorSelectorCommand = new RelayCommand<ColorSelector>(OpenColorSelector);
        }

        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }

        public CompositionService CompositionService { get; set; }
        public Counter ResolutionX { get; set; }
        public Counter ResolutionY { get; set; }
        public ColorSelector From { get; set; }
        public ColorSelector To { get; set; }
        public Slider Gamma { get; set; }
        public ToggleButton Horizontal { get; set; }
        public Transform2D Transform2D { get; set; }


        public void OpenColorSelector(ColorSelector colorSelector)
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, colorSelector);
        }


        public IModel GetModel()
        {
            GradientModel gradientModel = new GradientModel();

            gradientModel.ResolutionX = (CounterModel)ResolutionX.GetModel();
            gradientModel.ResolutionY = (CounterModel)ResolutionY.GetModel();
            gradientModel.From = (ColorSelectorModel)From.GetModel();
            gradientModel.To = (ColorSelectorModel)To.GetModel();
            gradientModel.Gamma = (SliderModel)Gamma.GetModel();
            gradientModel.Horizontal = (ToggleButtonModel)Horizontal.GetModel();
            gradientModel.Transform2D = (Transform2DModel)Transform2D.GetModel();
            return gradientModel;
        }

        public void SetViewModel(IModel model)
        {
            GradientModel gradientModel = model as GradientModel;
            ResolutionX.SetViewModel(gradientModel.ResolutionX);
            ResolutionY.SetViewModel(gradientModel.ResolutionY);
            From.SetViewModel(gradientModel.From);
            To.SetViewModel(gradientModel.To);
            Gamma.SetViewModel(gradientModel.Gamma);
            Horizontal.SetViewModel(gradientModel.Horizontal);
            Transform2D.SetViewModel(gradientModel.Transform2D);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
