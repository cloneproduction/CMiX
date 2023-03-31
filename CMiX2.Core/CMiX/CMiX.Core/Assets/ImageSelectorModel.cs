// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Assets
{
    public class ImageSelectorModel : IModel
    {
        public ImageSelectorModel()
        {
            ID = Guid.NewGuid();
        }
        public Guid ID { get; set; }
        public IAssetModel SelectedAsset { get; set; }
    }
}
