// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetImage : ObservableObject, IAsset
    {
        public AssetImage()
        {

        }

        public AssetImage(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public AssetImage(IAssetModel assetModel)
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

        private bool _isRoot = false;
        public bool IsRoot
        {
            get => _isRoot;
            set => SetProperty(ref _isRoot, value);
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            IAssetModel assetModel = new AssetImageModel();

            assetModel.Name = this.Name;
            assetModel.Path = this.Path;
            assetModel.Ponderation = this.Ponderation;

            return assetModel;
        }
    }
}
