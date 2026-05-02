// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Assets
{
    public class AssetRepository : ObservableObject
    {
        public SortableObservableCollection<Video> Videos { get; } = new();
        public SortableObservableCollection<ImageAsset> Images { get; } = new();
        public SortableObservableCollection<Geometry> Geometries { get; } = new();

        private readonly Dictionary<Type, Action<IAsset>> typeToAddAction;

        public AssetRepository()
        {
            typeToAddAction = new Dictionary<Type, Action<IAsset>>
            {
                { typeof(Video), asset => Videos.Add((Video)asset) },
                { typeof(ImageAsset), asset => Images.Add((ImageAsset)asset) },
                { typeof(Geometry), asset => Geometries.Add((Geometry)asset) }
            };
        }

        public void Add(IAsset asset)
        {
            typeToAddAction.FirstOrDefault(kvp => kvp.Key.IsInstanceOfType(asset))
                           .Value?.Invoke(asset);
        }

        public IAsset FindByPath(string path)
        {
            return Videos.FirstOrDefault(a => a.FilePath == path) as IAsset
                ?? Images.FirstOrDefault(a => a.FilePath == path) as IAsset
                ?? Geometries.FirstOrDefault(a => a.FilePath == path) as IAsset;
        }
    }
}
