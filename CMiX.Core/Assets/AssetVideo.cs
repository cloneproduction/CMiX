// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels.Assets
{
    public class AssetVideo : ObservableObject, IAssetImage
    {
        public AssetVideo()
        {

        }

        public AssetVideo(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public AssetVideo(IAssetModel assetModel)
        {
            this.Name = assetModel.Name;
            this.Path = assetModel.Path;
            this.Ponderation = assetModel.Ponderation;
        }


        private bool _fileExist;
        public bool FileExist
        {
            get => _fileExist;
            set => SetProperty(ref _fileExist, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _ponderation;
        public string Ponderation
        {
            get => _ponderation;
            set => SetProperty(ref _ponderation, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private string _path;
        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }



        public IModel GetModel()
        {
            IAssetModel assetModel = new AssetVideoModel();

            assetModel.Name = this.Name;
            assetModel.Path = this.Path;
            assetModel.Ponderation = this.Ponderation;

            return assetModel;
        }
    }
}
