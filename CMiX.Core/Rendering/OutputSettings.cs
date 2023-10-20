// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings()
        {
            Resolution = new Integer2(1920, 1080);
            BackgroundColor = new ColorValue(Color.FromArgb(255, 0, 0, 0));
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public ColorValue BackgroundColor { get; set; }
    }
}
