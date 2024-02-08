// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class Gradient : ObservableObject, ITextureSource
    {
        public Gradient(Integer2 resolution, GenericValue<string> from, GenericValue<string> to, GenericValue<float> gamma, GenericValue<bool> horizontal)
        {
            Resolution = resolution; // new Integer2(512, 512);
            From = from; // new ColorValue(Color.FromArgb(255, 255, 255, 255));
            To = to; // new ColorValue(Color.FromArgb(255, 0, 0, 0));
            Gamma = gamma; // new GenericValue<float>(2.2f);
            Horizontal = horizontal;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public GenericValue<string> From { get; set; }
        public GenericValue<string> To { get; set; }
        public GenericValue<float> Gamma { get; set; }
        public GenericValue<bool> Horizontal { get; set; }
    }
}
