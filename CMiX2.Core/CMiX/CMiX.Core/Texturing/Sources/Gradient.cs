// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient(GradientModel gradientModel, CompositionService compositionService)
        {
            ID = gradientModel.ID;
            CompositionService = compositionService;
            Resolution = new Integer2(gradientModel.Resolution);
            From = new ColorSelector(gradientModel.From);
            To = new ColorSelector(gradientModel.To);
            Gamma = new FloatValue(gradientModel.Gamma);
            Horizontal = new BooleanValue(gradientModel.Horizontal);
        }

        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }

        public CompositionService CompositionService { get; set; }
        public Integer2 Resolution { get; set; }
        public ColorSelector From { get; set; }
        public ColorSelector To { get; set; }
        public FloatValue Gamma { get; set; }
        public BooleanValue Horizontal { get; set; }


        public IModel GetModel()
        {
            GradientModel gradientModel = new GradientModel();

            gradientModel.Resolution = (Integer2Model)Resolution.GetModel();
            gradientModel.From = (ColorSelectorModel)From.GetModel();
            gradientModel.To = (ColorSelectorModel)To.GetModel();
            gradientModel.Gamma = (FloatValueModel)Gamma.GetModel();
            gradientModel.Horizontal = (BooleanValueModel)Horizontal.GetModel();
            return gradientModel;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
