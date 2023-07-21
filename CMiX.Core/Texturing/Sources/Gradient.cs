// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient()
        {
            Resolution = new Integer2(512, 512);
            From = new ColorSelector(Color.FromArgb(255, 255, 255, 255));
            To = new ColorSelector(Color.FromArgb(255, 0, 0, 0));
            Gamma = new FloatValue(2.2f);
            Horizontal = new BooleanValue();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public ColorSelector From { get; set; }
        public ColorSelector To { get; set; }
        public FloatValue Gamma { get; set; }
        public BooleanValue Horizontal { get; set; }
    }
}
