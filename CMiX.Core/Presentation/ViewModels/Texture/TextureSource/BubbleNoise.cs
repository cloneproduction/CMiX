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
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise(BubbleNoiseModel bubbleNoiseModel, CompositionService compositionService)
        {
            ID = bubbleNoiseModel.ID;
            CompositionService = compositionService;

            ResolutionX = new Counter(bubbleNoiseModel.ResolutionX);
            ResolutionY = new Counter(bubbleNoiseModel.ResolutionY);
            Speed = new Slider(nameof(Speed), bubbleNoiseModel.Speed);
            Frequency = new Slider(nameof(Frequency), bubbleNoiseModel.Frequency);
            Contrast = new Slider(nameof(Contrast), bubbleNoiseModel.Contrast);
            BackgroundColor = new ColorSelector(bubbleNoiseModel.BackgroundColor);
            BubbleColor = new ColorSelector(bubbleNoiseModel.BubbleColor);

            OpenColorSelectorCommand = new RelayCommand<ColorSelector>(OpenColorSelector);
        }


        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }


        public CompositionService CompositionService { get; set; }
        public Counter ResolutionX { get; set; }
        public Counter ResolutionY { get; set; }
        public Slider Speed { get; set; }
        public Slider Frequency { get; set; }
        public Slider Contrast { get; set; }


        public ColorSelector BackgroundColor { get; set; }
        public ColorSelector BubbleColor { get; set; }


        public void OpenColorSelector(ColorSelector colorSelector)
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, colorSelector);
        }


        public IModel GetModel()
        {
            BubbleNoiseModel gradientModel = new BubbleNoiseModel();

            gradientModel.ResolutionX = (CounterModel)ResolutionX.GetModel();
            gradientModel.ResolutionY = (CounterModel)ResolutionY.GetModel();
            gradientModel.Speed = (SliderModel)Speed.GetModel();
            gradientModel.Frequency = (SliderModel)Frequency.GetModel();
            gradientModel.Contrast = (SliderModel)Contrast.GetModel();
            gradientModel.BackgroundColor = (ColorSelectorModel)BackgroundColor.GetModel();
            gradientModel.BubbleColor = (ColorSelectorModel)BubbleColor.GetModel();

            return gradientModel;
        }

        public void SetViewModel(IModel model)
        {
            BubbleNoiseModel gradientModel = model as BubbleNoiseModel;
            ResolutionX.SetViewModel(gradientModel.ResolutionX);
            ResolutionY.SetViewModel(gradientModel.ResolutionY);
            BackgroundColor.SetViewModel(gradientModel.BackgroundColor);
            BubbleColor.SetViewModel(gradientModel.BubbleColor);
            Speed.SetViewModel(gradientModel.Speed);
            Frequency.SetViewModel(gradientModel.Frequency);
            Contrast.SetViewModel(gradientModel.Contrast);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
