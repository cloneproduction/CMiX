// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Assets
{
    public class AssetRepository : ObservableObject
    {
        public SortableObservableCollection<Video> Videos { get; } = new();
        public SortableObservableCollection<Image> Images { get; } = new();
        public SortableObservableCollection<Geometry> Geometries { get; } = new();

        private readonly Dictionary<Type, Action<IAsset>> typeToAddAction;

        public AssetRepository()
        {
            typeToAddAction = new Dictionary<Type, Action<IAsset>>
            {
                { typeof(Video), asset => Videos.Add((Video)asset) },
                { typeof(Image), asset => Images.Add((Image)asset) },
                { typeof(Geometry), asset => Geometries.Add((Geometry)asset) }
            };
        }

        public void Add(IAsset asset)
        {
            typeToAddAction.FirstOrDefault(kvp => kvp.Key.IsInstanceOfType(asset))
                           .Value?.Invoke(asset);
        }
    }
}
