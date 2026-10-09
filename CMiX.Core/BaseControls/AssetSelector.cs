// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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

        public IAsset CreateAssetFromPath(string path)
        {
            // A folder is an image sequence.
            if (Directory.Exists(path)) return new ImageSequence(path);
            if (!File.Exists(path)) return null;
            if (!AssetTypes.TryResolve(path, out var kind)) return null;

            return kind switch
            {
                AssetKind.Image => new ImageAsset(path),
                AssetKind.Geometry => new Geometry(path),
                AssetKind.Video => new Video(path),
                _ => null
            };
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
            FilePath.ID = m.FilePath.ID;

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
