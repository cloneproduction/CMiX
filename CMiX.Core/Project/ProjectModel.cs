// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Assets;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Compositing
{
    public record ProjectModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
        public ObservableCollection<IAssetModel> AssetModels { get; set; } = new();
        public ObservableCollection<IAssetModel> AssetModelsFlatten { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
    }
}
