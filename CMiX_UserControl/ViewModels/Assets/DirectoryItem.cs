﻿using System.Collections.Specialized;
using System.ComponentModel;
using CMiX.MVVM.Resources;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;

namespace CMiX.Studio.ViewModels
{
    public class DirectoryItem : ViewModel, IDirectory
    {

        #region CONSTRUCTORS
        public DirectoryItem()
        {
            Assets = new SortableObservableCollection<IAssets>();
            Assets.CollectionChanged += CollectionChanged;
            IsExpanded = false;
            IsSelected = false;
        }
        public DirectoryItem(string name)
        {
            Name = name;
            Assets = new SortableObservableCollection<IAssets>();
            Assets.CollectionChanged += CollectionChanged;
            IsExpanded = false;
            IsSelected = false;
        }
        #endregion

        public SortableObservableCollection<IAssets> Assets { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        private string _ponderation = "a";
        public string Ponderation
        {
            get => _ponderation;
            set => _ponderation = value;
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetAndNotify(ref _isRenaming, value);
        }

        private bool _isRoot = false;
        public bool IsRoot
        {
            get => _isRoot;
            set => SetAndNotify(ref _isRoot, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetAndNotify(ref _isExpanded, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndNotify(ref _isSelected, value);
        }

        public void AddAsset(IAssets asset)
        {
            Assets.Add(asset);
            SortAssets();
        }

        public void RemoveAsset(IAssets asset)
        {
            if (Assets.Contains(asset))
                Assets.Remove(asset);
        }

        public void Rename()
        {
            IsRenaming = true;
        }

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

        public IAssetModel GetModel()
        {
            DirectoryAssetModel directoryAssetModel = new DirectoryAssetModel();
            directoryAssetModel.Name = Name;
            foreach (var asset in Assets)
            {
                directoryAssetModel.AssetModels.Add(asset.GetModel());
            }
            return directoryAssetModel;
        }

        public void SetViewModel(IAssetModel model)
        {
            Name = model.Name;
            Assets.Clear();
            foreach (var assetModel in model.AssetModels)
            {
                IAssets asset = null;

                if(assetModel is DirectoryAssetModel)
                    asset = new DirectoryItem();
                else if(assetModel is GeometryAssetModel)
                    asset = new GeometryItem();
                else if(assetModel is TextureAssetModel)
                    asset = new TextureItem();

                asset.SetViewModel(assetModel);
                Assets.Add(asset);
            }
            SortAssets();
        }
    }
}