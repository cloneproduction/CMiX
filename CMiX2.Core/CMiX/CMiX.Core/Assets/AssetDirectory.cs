// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetDirectory : ObservableObject, IAsset, IDisposable
    {
        public AssetDirectory()
        {
            Assets = new SortableObservableCollection<IAsset>();
            Assets.CollectionChanged += CollectionChanged;
            IsExpanded = false;
            IsSelected = false;
        }

        public AssetDirectory(string name) : this()
        {
            Name = name;
        }

        public SortableObservableCollection<IAsset> Assets { get; set; }

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

        private string _ponderation = "a";
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


        public void AddAsset(IAsset asset)
        {
            Assets.Add(asset);
            SortAssets();
        }

        public void RemoveAsset(IAsset asset)
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


        public void Dispose()
        {
            foreach (var asset in this.Assets)
            {
                if (asset is IDisposable)
                    ((IDisposable)asset).Dispose();
            }
            this.Assets.Clear();
        }

        public IModel GetModel()
        {
            IAssetModel directoryAssetModel = new AssetDirectoryModel() as IAssetModel;

            directoryAssetModel.Name = this.Name;
            foreach (var asset in this.Assets)
            {
                directoryAssetModel.AssetModels.Add(asset.GetModel() as IAssetModel);
            }
            return directoryAssetModel as IModel;
        }
    }
}
