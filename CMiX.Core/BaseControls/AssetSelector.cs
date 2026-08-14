// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class AssetSelector : ObservableRecipient, IControl
    {
        public AssetSelector(GenericValue<string> filePath,
                             AssetRepository assetRepository)
        {
            FilePath = filePath;
            AssetRepository = assetRepository;

            FilePath.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GenericValue<string>.Value))
                    OnPropertyChanged(nameof(Asset));
            };
        }

        public Guid ID { get; set; }
        public GenericValue<string> FilePath { get; set; }
        public AssetRepository AssetRepository { get; set; }

        public IAsset Asset
        {
            get
            {
                var result = AssetRepository.FindByPath(FilePath.Value);
                return result;
            }
            set
            {
                if (value != null)
                    FilePath.Value = value.FilePath;
            }
        }

        private static readonly Dictionary<string, Func<string, IAsset>> AssetFactories = new()
    {
        { "PNG", path => new ImageAsset(path) },
        { "JPG", path => new ImageAsset(path) },
        { "JPEG", path => new ImageAsset(path) },
        { "OBJ", path => new Geometry(path) },
        { "FBX", path => new Geometry(path) },
        { "MOV", path => new Video(path) }
    };

        public IAsset CreateAssetFromPath(string path)
        {
            if (!File.Exists(path)) return null;
            string ext = Path.GetExtension(path).ToUpperInvariant().TrimStart('.');
            return AssetFactories.TryGetValue(ext, out var factory) ? factory(path) : null;
        }

        public void SetAssetFromPath(string filePath)
        {
            var existing = AssetRepository.FindByPath(filePath);
            if (existing != null) { Asset = existing; return; }
            var asset = CreateAssetFromPath(filePath);
            if (asset == null) return;
            AssetRepository.Add(asset);
            Asset = asset;
        }

        public IControlModel ToModel() => new AssetSelectorModel
        {
            ID = ID,
            FilePath = (GenericValueModel<string>)FilePath.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (AssetSelectorModel)model;
            ID = m.ID;

            if (!string.IsNullOrEmpty(m.FilePath.Value))
                FilePath.FromModel(m.FilePath);

            if (string.IsNullOrEmpty(FilePath.Value)) return;

            var existing = AssetRepository.FindByPath(FilePath.Value);
            if (existing != null) { Asset = existing; return; }

            var asset = CreateAssetFromPath(FilePath.Value);
            if (asset == null) return;
            AssetRepository.Add(asset);
            Asset = asset;
        }
    }
}
