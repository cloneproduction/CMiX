// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoIn : ObservableRecipient, IControl
    {
        public VideoIn()
        {
            SizeX = new IntegerValue(0);
            SizeY = new IntegerValue(0);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public IntegerValue SizeX { get; set; }
        public IntegerValue SizeY { get; set; }
    }
}
