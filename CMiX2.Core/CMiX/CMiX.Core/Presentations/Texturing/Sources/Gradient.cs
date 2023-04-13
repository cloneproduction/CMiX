// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Texturing.Sources
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient(GradientModel gradientModel, CompositionService compositionService)
        {
            ID = gradientModel.ID;
            CompositionService = compositionService;
            Resolution = new Integer2(gradientModel.Resolution, compositionService);
            From = new ColorSelector(gradientModel.From, compositionService);
            To = new ColorSelector(gradientModel.To, compositionService);
            Gamma = new FloatValue(gradientModel.Gamma, compositionService);
            Horizontal = new BooleanValue(gradientModel.Horizontal, compositionService);
        }

        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }

        public CompositionService CompositionService { get; set; }
        public Integer2 Resolution { get; set; }
        public ColorSelector From { get; set; }
        public ColorSelector To { get; set; }
        public FloatValue Gamma { get; set; }
        public BooleanValue Horizontal { get; set; }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
