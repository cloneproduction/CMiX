// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Media.Media3D;
using CMiX.Core.Assets;
using CMiX.Core.BaseControls.CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using Image = CMiX.Core.Assets.Image;

namespace CMiX.Core.BaseControls
{
    public class AssetSelector : ObservableRecipient, IControl, IDropTarget
    {
        public AssetSelector(GenericValue<string> filePath,
                             AssetRepository assetRepository)
        {
            FilePath = filePath;
            AssetRepository = assetRepository;
        }

        public Guid ID { get; set; }
        public GenericValue<string> FilePath { get; set; }
        public AssetRepository AssetRepository { get; set; }

        private IAsset _asset;
        public IAsset Asset
        {
            get => _asset;
            set
            {
                _asset = value;
                OnPropertyChanged();
                if (value != null)
                    FilePath.Value = value.FilePath;
            }
        }

        private static readonly Dictionary<string, Func<string, IAsset>> AssetFactories = new()
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
            if (!File.Exists(path)) return null;
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
                                   .Cast<string>()
                                   .Where(File.Exists)
                                   .Select(CreateAssetFromPath)
                                   .Where(a => a != null)
                                   .ToList();

            foreach (var asset in assets)
                AssetRepository.Add(asset);

            var first = assets.FirstOrDefault();
            if (first == null) return;

            Asset = first;
            FilePath.Value = first.FilePath;
        }

        public IControlModel ToModel() => new AssetSelectorModel
        {
            ID = ID,
            FilePath = (GenericValueModel<string>)FilePath.ToModel(),
            AssetType = Asset?.GetType().Name ?? string.Empty
        };

        public void FromModel(IControlModel model)
        {
            var m = (AssetSelectorModel)model;
            ID = m.ID;
            FilePath.FromModel(m.FilePath);

            if (string.IsNullOrEmpty(m.FilePath.Value)) return;

            IAsset asset = m.AssetType switch
            {
                "Image" => new Image(),
                "Video" => new Video(),
                "Geometry" => new Geometry(),
                _ => null
            };

            if (asset == null) return;
            asset.FilePath = m.FilePath.Value;
            AssetRepository.Add(asset);
            Asset = asset;
        }
    }
}
