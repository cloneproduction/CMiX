// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoIn : ObservableRecipient, IControl
    {
        public VideoIn(GenericValue<int> sizeX, GenericValue<int> sizeZ)
        {
            SizeX = sizeX; // new GenericValue<int>(0);
            SizeY = sizeZ; // new GenericValue<int>(0);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> SizeX { get; set; }
        public GenericValue<int> SizeY { get; set; }
    }
}
