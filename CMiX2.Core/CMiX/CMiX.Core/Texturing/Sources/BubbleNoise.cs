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
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise(BubbleNoiseModel bubbleNoiseModel, CompositionService compositionService)
        {
            ID = bubbleNoiseModel.ID;
            CompositionService = compositionService;

            Resolution = new Integer2(bubbleNoiseModel.Resolution);
            Speed = new FloatValue(bubbleNoiseModel.Speed);
            Frequency = new FloatValue(bubbleNoiseModel.Frequency);
            Contrast = new FloatValue(bubbleNoiseModel.Contrast);
            BackgroundColor = new ColorSelector(bubbleNoiseModel.BackgroundColor);
            BubbleColor = new ColorSelector(bubbleNoiseModel.BubbleColor);

            OpenColorSelectorCommand = new RelayCommand<ColorSelector>(OpenColorSelector);
        }


        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }


        public CompositionService CompositionService { get; set; }
        public Integer2 Resolution { get; set; }
        public FloatValue Speed { get; set; }
        public FloatValue Frequency { get; set; }
        public FloatValue Contrast { get; set; }


        public ColorSelector BackgroundColor { get; set; }
        public ColorSelector BubbleColor { get; set; }


        public void OpenColorSelector(ColorSelector colorSelector)
        {
            //CompositionService.DialogService.Show<ColorSelectorWindow>(this, colorSelector);
        }


        public IModel GetModel()
        {
            BubbleNoiseModel gradientModel = new BubbleNoiseModel();

            gradientModel.Resolution = (Integer2Model)Resolution.GetModel();
            gradientModel.Speed = (FloatValueModel)Speed.GetModel();
            gradientModel.Frequency = (FloatValueModel)Frequency.GetModel();
            gradientModel.Contrast = (FloatValueModel)Contrast.GetModel();
            gradientModel.BackgroundColor = (ColorSelectorModel)BackgroundColor.GetModel();
            gradientModel.BubbleColor = (ColorSelectorModel)BubbleColor.GetModel();

            return gradientModel;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
