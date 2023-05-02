// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;

namespace CMiX.Core.Assets
{
    public class AssetModel : IAssetModel
    {
        public AssetModel()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Ponderation { get; set; }
        public string Path { get; set; }

        public ObservableCollection<IAssetModel> AssetModels { get; set; }
    }
}
