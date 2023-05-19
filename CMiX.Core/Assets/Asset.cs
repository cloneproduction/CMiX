// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels.Assets
{
    public partial class Asset : ObservableObject, IAsset
    {
        public Asset()
        {

        }

        public Asset(string path)
        {
            this.FilePath = Path.GetFullPath(path);
        }

        public Guid ID { get; set; }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public bool FileExist
        {
            get => File.Exists(FilePath);
        }

        public string Name
        {
            get => Path.GetFileNameWithoutExtension(FilePath);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
