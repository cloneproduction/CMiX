// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetDirectory : Asset, IDisposable
    {
        public AssetDirectory()
        {
            Assets = new SortableObservableCollection<Asset>();
            Assets.CollectionChanged += CollectionChanged;
            IsExpanded = false;
            IsSelected = false;
        }

        public AssetDirectory(string name)
        {
            Name = name;
            Assets = new SortableObservableCollection<Asset>();
            Assets.CollectionChanged += CollectionChanged;
            IsExpanded = false;
            IsSelected = false;
        }

        public SortableObservableCollection<Asset> Assets { get; set; }


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


        public void AddAsset(Asset asset)
        {
            Assets.Add(asset);
            SortAssets();
        }


        public void RemoveAsset(Asset asset)
        {
            if (Assets.Contains(asset))
                Assets.Remove(asset);
        }

        public void Rename() => IsRenaming = true;

        public void SortAssets()
        {
            Assets.Sort(c => c.Name);
            Assets.Sort(c => c.Ponderation.ToString());
        }

        public void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;
            }
        }

        public void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name))
                SortAssets();
        }


        //public void SetViewModel(IAssetModel model)
        //{
        //    Name = model.Name;
        //    Assets.Clear();
        //    foreach (var assetModel in model.AssetModels)
        //    {
        //        Asset asset = null;

        //        if(assetModel is AssetDirectoryModel)
        //            asset = new AssetDirectory();
        //        else if(assetModel is AssetGeometryModel)
        //            asset = new AssetGeometry();
        //        else if(assetModel is AssetTextureModel)
        //            asset = new AssetTexture();

        //        if(asset != null)
        //        {
        //            asset.SetViewModel(assetModel);
        //            Assets.Add(asset);
        //        }
        //    }
        //    SortAssets();
        //}

        public void Dispose()
        {
            foreach (var asset in this.Assets)
            {
                if (asset is IDisposable)
                    ((IDisposable)asset).Dispose();
            }
            this.Assets.Clear();
        }

        public override IModel GetModel()
        {
            IAssetModel directoryAssetModel = new AssetDirectoryModel() as IAssetModel;

            directoryAssetModel.Name = this.Name;
            foreach (var asset in this.Assets)
            {
                directoryAssetModel.AssetModels.Add(asset.GetModel() as IAssetModel);
            }
            return directoryAssetModel as IModel;
        }

        public override void SetViewModel(IModel model)
        {
            AssetDirectoryModel assetDirectoryModel = model as AssetDirectoryModel;
            this.Name = assetDirectoryModel.Name;

            this.Assets.Clear();
            foreach (var assetModel in assetDirectoryModel.AssetModels)
            {
                Asset asset = null;

                if (assetModel is AssetDirectoryModel)
                    asset = new AssetDirectory();
                else if (assetModel is AssetGeometryModel)
                    asset = new AssetGeometry();
                else if (assetModel is AssetTextureModel)
                    asset = new AssetTexture();

                if (asset != null)
                {
                    asset.SetViewModel(assetModel);
                    this.Assets.Add(asset);
                }
            }
            this.SortAssets();
        }
    }
}
