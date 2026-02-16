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

        public IControlModel ToModel() => this.ToModel();
        public Guid ID { get; set; }
        public GenericValue<IAsset> Asset { get; set; }
        public AssetRepository AssetRepository { get; set; }

        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            var dragInfo = dropInfo.DragInfo;

            if (dataObject != null && dataObject.ContainsFileDropList())
                dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;

            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                List<IAsset> assets = new List<IAsset>();
                foreach (string str in dataObject.GetFileDropList())
                {
                    if (File.Exists(str))
                    {
                        var asset = CreateAssetFromPath(str);
                        AssetRepository.Add(asset);
                        assets.Add(asset);
                    }

                    //if (Directory.Exists(str))
                    //    CreateAssetFromDirectory(new DirectoryInfo(str));
                }

                Asset.Value = assets.FirstOrDefault();
            }
        }

        private IAsset CreateAssetFromPath(string path)
        {
            IAsset asset = null;

            if (File.Exists(path))
            {
                string fileType = Path.GetExtension(path).ToUpper().TrimStart('.');

                if (fileType == "PNG" || fileType == "JPG" || fileType == "JPEG")
                    asset = new Image(path);

                if (fileType == "OBJ" || fileType == "FBX")
                    asset = new Geometry(path);

                if (fileType == "MOV")
                    asset = new Video(path);
            }

            return asset;
        }
    }
}
