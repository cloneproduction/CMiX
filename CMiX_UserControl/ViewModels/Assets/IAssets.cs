﻿using CMiX.MVVM.Models;


namespace CMiX.Studio.ViewModels
{
    public interface IAssets
    {
        string Path { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
        string Ponderation { get; set; }

        //IAssetModel GetModel();
        void SetViewModel(IAssetModel assetModel);
    }
}