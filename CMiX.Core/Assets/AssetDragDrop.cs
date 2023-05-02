// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.ViewModels.Assets
{
    public class AssetDragDrop
    {
        public AssetDragDrop()
        {
            SourceCollection = new ObservableCollection<IAsset>();
        }

        public IAsset DragObject { get; set; }
        public ObservableCollection<IAsset> SourceCollection { get; set; }
    }
}
