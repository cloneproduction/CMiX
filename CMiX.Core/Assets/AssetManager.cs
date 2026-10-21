// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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

        [RelayCommand]
        public void AddAsset()
        {
            if (SelectedItems.Take(2).Count() == 1 && SelectedItems.FirstOrDefault() is AssetDirectory assetDirectory)
            {
                assetDirectory.IsExpanded = true;
                assetDirectory.AddAsset(new AssetDirectory("New Folder"));
            }
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
