// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using CMiX.Core.Assets;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.BaseControls
{
    public class AssetSelector : ObservableRecipient, IControl, IDropTarget
    {
        public AssetSelector(GenericValue<IAsset> asset,
                             AssetRepository assetRepository)
        {
            Asset = asset;
            AssetRepository = assetRepository;
        }

        public Guid ID { get; set; }
        public GenericValue<IAsset> Asset { get; set; }
        public AssetRepository AssetRepository { get; set; }

        private static readonly Dictionary<string, Func<string, IAsset>> AssetFactories =
            new()
            {
                { "PNG", path => new Image(path) },
                { "JPG", path => new Image(path) },
                { "JPEG", path => new Image(path) },
                { "OBJ", path => new Geometry(path) },
                { "FBX", path => new Geometry(path) },
                { "MOV", path => new Video(path) }
            };

        private IAsset CreateAssetFromPath(string path)
        {
            if (!File.Exists(path))
                return null;

            string ext = Path.GetExtension(path).ToUpperInvariant().TrimStart('.');
            return AssetFactories.TryGetValue(ext, out var factory) ? factory(path) : null;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject dataObject && dataObject.ContainsFileDropList())
                dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not DataObject dataObject || !dataObject.ContainsFileDropList())
                return;

            var assets = dataObject.GetFileDropList()
                                   .Cast<string>()// Convert StringCollection to IEnumerable<string>
                                   .Where(File.Exists)
                                   .Select(CreateAssetFromPath)
                                   .ToList(); 

            foreach (var asset in assets)
                AssetRepository.Add(asset);

            Asset.Value = assets.FirstOrDefault();
        }
    }
}
