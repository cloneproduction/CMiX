// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public interface IAsset
    {
        bool FileExist { get; set; }
        string Name { get; set; }
        string Ponderation { get; set; }
        bool IsRenaming { get; set; }
        bool IsSelected { get; set; }
        string Path { get; set; }

        IModel GetModel();
        void SetViewModel(IModel model);

        //private string _name;
        //public string Name
        //{
        //    get => _name;
        //    set => SetProperty(ref _name, value);
        //}

        //private string _ponderation = "aa";
        //public string Ponderation
        //{
        //    get => _ponderation;
        //    set => _ponderation = value;
        //}

        //private bool _isRenaming = false;
        //public bool IsRenaming
        //{
        //    get => _isRenaming;
        //    set => SetProperty(ref _isRenaming, value);
        //}

        //private bool _isSelected = false;
        //public bool IsSelected
        //{
        //    get => _isSelected;
        //    set => SetProperty(ref _isSelected, value);
        //}

        //private string _path;
        //public string Path
        //{
        //    get => _path;
        //    set
        //    {
        //        SetProperty(ref _path, value);
        //        OnPropertyChanged(nameof(FileExist));
        //    }
        //}


        //private bool _fileExist;
        //public bool FileExist
        //{
        //    get => File.Exists(Path);
        //    set => SetProperty(ref _fileExist, value);
        //}


    }
}
