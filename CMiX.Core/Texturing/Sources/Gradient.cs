// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Security.Policy;
using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient(Integer2 resolution, ColorValue from, ColorValue to, FloatValue gamma, BooleanValue horizontal)
        {
            Resolution = resolution; // new Integer2(512, 512);
            From = from; // new ColorValue(Color.FromArgb(255, 255, 255, 255));
            To = to; // new ColorValue(Color.FromArgb(255, 0, 0, 0));
            Gamma = gamma; // new FloatValue(2.2f);
            Horizontal = horizontal;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public ColorValue From { get; set; }
        public ColorValue To { get; set; }
        public FloatValue Gamma { get; set; }
        public BooleanValue Horizontal { get; set; }
    }
}
