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
            FilePath = path;
        }

        public AssetVideo(IAssetModel assetModel)
        {
            this.Name = assetModel.Name;
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

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
    }
}
