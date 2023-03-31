// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Assets
{
    public interface IAssetModel : IModel
    {
        string Path { get; set; }
        string Name { get; set; }
        string Ponderation { get; set; }

        ObservableCollection<IAssetModel> AssetModels { get; set; }
    }
}