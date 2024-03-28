// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoIn : ObservableRecipient, IControl, IPrefab
    {
        public VideoIn(PrefabService prefabService, 
                       GenericValue<int> sizeX, 
                       GenericValue<int> sizeZ)
        {
            PrefabService = prefabService;
            SizeX = sizeX;
            SizeY = sizeZ;
        }
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> SizeX { get; set; }
        public GenericValue<int> SizeY { get; set; }
        public PrefabService PrefabService { get; set; }
    }
}
