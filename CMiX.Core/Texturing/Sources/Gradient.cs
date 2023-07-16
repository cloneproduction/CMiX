// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient()
        {
            ID = Guid.NewGuid();
            Resolution = new Integer2();
            From = new ColorSelector();
            To = new ColorSelector();
            Gamma = new FloatValue();
            Horizontal = new BooleanValue();
        }

        public ICommand OpenColorSelectorCommand { get; set; }
        public Guid ID { get; set; }
        public Integer2 Resolution { get; set; }
        public ColorSelector From { get; set; }
        public ColorSelector To { get; set; }
        public FloatValue Gamma { get; set; }
        public BooleanValue Horizontal { get; set; }
    }
}
