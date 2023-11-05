// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(Integer2 resolution, ColorValue backgroundColor)
        {
            Resolution = resolution;
            BackgroundColor = backgroundColor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public ColorValue BackgroundColor { get; set; }
    }
}
