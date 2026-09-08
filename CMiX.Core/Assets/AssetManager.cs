// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Assets
{
    public partial class AssetManager : ObservableRecipient, IControl
    {
        public AssetManager(AssetRepository assetRepository)
        {
            AssetRepository = assetRepository;
        }

        public Guid ID { get; set; }
        public AssetRepository AssetRepository { get; set; }
        public ObservableCollection<IAsset> SelectedItems { get; set; } = new();

        public void RenameAsset()
        {
            if (SelectedItems.Take(2).Count() == 1 && SelectedItems.First() is AssetDirectory assetDirectory)
                assetDirectory.Rename();
        }

        [RelayCommand]
        public void AddAsset()
        {
            if (SelectedItems.Take(2).Count() == 1 && SelectedItems.FirstOrDefault() is AssetDirectory assetDirectory)
            {
                assetDirectory.IsExpanded = true;
                assetDirectory.AddAsset(new AssetDirectory("New Folder"));
            }
        }

        [RelayCommand]
        private void DeleteAssets() { }

        public void RemoveItemFromDirectory(AssetDirectory directory)
        {
            var toBeRemoved = new List<IAsset>();
            foreach (var asset in directory.Assets)
            {
                if (asset is AssetDirectory subDir)
                    RemoveItemFromDirectory(subDir);
                toBeRemoved.Add(asset);
            }
            foreach (var item in toBeRemoved)
                directory.Assets.Remove(item);
        }

        public void DeleteSelectedAssets(ObservableCollection<IAsset> assets)
        {
            var toBeRemoved = new List<IAsset>();
            foreach (var asset in assets)
            {
                if (asset is AssetDirectory assetDirectory)
                {
                    if (assetDirectory.IsSelected)
                    {
                        toBeRemoved.Add(asset);
                        RemoveItemFromDirectory(assetDirectory);
                        return;
                    }
                    DeleteSelectedAssets(assetDirectory.Assets);
                }
            }
            foreach (var item in toBeRemoved)
                assets.Remove(item);
            SelectedItems.Clear();
        }

        public void CreateAssetFromDirectory(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
                CreateAssetFromDirectory(directory);
            foreach (var file in directoryInfo.GetFiles())
                CreateAssetFromPath(file.FullName);
        }

        public List<string> EnumerateAssetFilePaths(DirectoryInfo directoryInfo)
        {
            var paths = new List<string>();
            foreach (var file in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                paths.Add(file.FullName);
            return paths;
        }

        public void CreateAssetFromPath(string path)
        {
            if (!File.Exists(path)) return;
            if (!AssetTypes.TryResolve(path, out var kind)) return;

            IAsset asset = kind switch
            {
                AssetKind.Image => new ImageAsset(path),
                AssetKind.Geometry => new Geometry(path),
                AssetKind.Video => new Video(path),
                _ => null
            };
            if (asset != null)
                AssetRepository.Add(asset);
        }

        public IControlModel ToModel() => new AssetManagerModel { ID = ID };
        public void FromModel(IControlModel model) => ID = ((AssetManagerModel)model).ID;
    }
}
