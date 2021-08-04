// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMiX.Core.Presentation.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetManager : ObservableRecipient, IDropTarget, IDragSource
    {
        public AssetManager(IDialogService dialogService)
        {
            AssetFactory = new AssetFactory();
            DialogService = dialogService;

            Assets = new SortableObservableCollection<IAsset>();
            TextureAssets = new SortableObservableCollection<AssetTexture>();
            GeometryAssets = new SortableObservableCollection<AssetGeometry>();

            SelectedItems = new ObservableCollection<IAsset>();
            SelectedItems.CollectionChanged += CollectionChanged;

            AddAssetCommand = new RelayCommand(AddAsset);
            DeleteAssetsCommand = new RelayCommand(DeleteAssets);
            RenameAssetCommand = new RelayCommand(RenameAsset);
            RelinkAssetsCommand = new RelayCommand(RelinkAssets);
        }

        private AssetFactory AssetFactory { get; set; }
        public IDialogService DialogService { get; set; }

        public ICommand RenameAssetCommand { get; set; }
        public ICommand AddAssetCommand { get; set; }
        public ICommand DeleteAssetsCommand { get; set; }
        public ICommand DeleteSelectedItemCommand { get; set; }
        public ICommand RelinkAssetsCommand { get; set; }


        private ObservableCollection<IAsset> _selectedItems;
        public ObservableCollection<IAsset> SelectedItems
        {
            get => _selectedItems;
            set => SetProperty(ref _selectedItems, value);
        }

        private AssetTypes _selectedAssetType = AssetTypes.Texture;
        public AssetTypes SelectedAssetType
        {
            get => _selectedAssetType;
            set => SetProperty(ref _selectedAssetType, value);
        }

        private bool _canAddAsset = false;
        public bool CanAddAsset
        {
            get => _canAddAsset;
            set => SetProperty(ref _canAddAsset, value);
        }

        private bool _canRenameAsset = false;
        public bool CanRenameAsset
        {
            get => _canRenameAsset;
            set => SetProperty(ref _canRenameAsset, value);
        }

        private bool _canDeleteAsset = false;
        public bool CanDeleteAsset
        {
            get => _canDeleteAsset;
            set => SetProperty(ref _canDeleteAsset, value);
        }

        private bool _canRelinkAsset = false;
        public bool CanRelinkAsset
        {
            get => _canRelinkAsset;
            set => SetProperty(ref _canRelinkAsset, value);
        }


        public SortableObservableCollection<IAsset> Assets { get; set; }

        public SortableObservableCollection<AssetTexture> TextureAssets { get; set; }
        public SortableObservableCollection<AssetGeometry> GeometryAssets { get; set; }


        public void RenameAsset()
        {
            if (SelectedItems.Take(2).Count() == 1)
            {
                if (SelectedItems.First() is AssetDirectory assetDirectory)
                    assetDirectory.Rename();
            }
        }

        public void RelinkAssets()
        {
            if (SelectedItems.Take(2).Count() == 1)
            {
                IAsset asset = SelectedItems.First();
                OpenFileDialogSettings settings = new OpenFileDialogSettings();

                if (asset is AssetTexture)
                    settings.Filter = "Image |*.jpg;*.jpeg;*.png;*.dds";
                else if (asset is AssetGeometry)
                    settings.Filter = "Geometry |*.fbx; *.obj";

                bool? success = DialogService.ShowOpenFileDialog(this, settings);
                if (success == true)
                {
                    asset.Path = settings.FileName;
                    asset.Name = Path.GetFileName(settings.FileName);
                }
            }
        }

        public void AddAsset()
        {
            if (SelectedItems.Take(2).Count() == 1 && SelectedItems.FirstOrDefault() is AssetDirectory assetDirectory)
            {
                assetDirectory.IsExpanded = true;
                assetDirectory.AddAsset(new AssetDirectory("New Folder"));
            }
        }


        private void DeleteAssets()
        {
            DeleteSelectedAssets(this.Assets);
        }


        public void RemoveItemFromDirectory(AssetDirectory directory)
        {
            var toBeRemoved = new List<IAsset>();
            foreach (var asset in directory.Assets)
            {
                if (asset is AssetDirectory)
                    RemoveItemFromDirectory(asset as AssetDirectory);

                toBeRemoved.Add(asset);
            }

            foreach (var item in toBeRemoved)
            {
                directory.Assets.Remove(item);
                this.Assets.Remove(item);
            }
        }


        public void DeleteSelectedAssets(ObservableCollection<IAsset> assets)
        {
            var toBeRemoved = new List<IAsset>();

            foreach (var asset in assets)
            {
                if (asset is AssetDirectory assetDirectory)
                {
                    if (assetDirectory.IsSelected)
                    {
                        toBeRemoved.Add(asset);
                        RemoveItemFromDirectory(assetDirectory);
                        return;
                    }

                    DeleteSelectedAssets(assetDirectory.Assets);
                }
            }

            foreach (var item in toBeRemoved)
            {
                assets.Remove(item);
            }

            SelectedItems.Clear();
        }


        private void CreateAssetFromDirectory(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                CreateAssetFromDirectory(directory);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                CreateAssetFromPath(file.FullName);
            }
        }


        private void CreateAssetFromPath(string path)
        {

            if (File.Exists(path))
            {
                string filePath = Path.GetFullPath(path);
                string fileType = Path.GetExtension(filePath).ToUpper().TrimStart('.');
                string fileName = Path.GetFileName(filePath);
                var asset = AssetFactory.CreateAsset(fileType, fileName, filePath);
                LoadAsset(asset);
            }

            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                CreateAssetFromDirectory(directoryInfo);
            }
        }


        private void LoadAsset(IAsset asset)
        {
            switch (asset)
            {
                case AssetGeometry assetGeometry:
                    GeometryAssets.Add(assetGeometry);
                    break;
                case AssetTexture assetTexture:
                    TextureAssets.Add(assetTexture);
                    break;
            }
        }


        public void SortAssets()
        {
            Assets.Sort(c => c.Name);
            Assets.Sort(c => c.Ponderation.ToString());
        }


        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            var dragInfo = dropInfo.DragInfo;

            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                //dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            }

            //if (dragInfo != null && dragInfo.SourceItem is IAsset)
            //{
            //    var targetItem = dropInfo.TargetItem;
            //    var vSourceItem = dropInfo.DragInfo.VisualSourceItem as TreeViewItem;
            //    var vSourceChild = vSourceItem.FindVisualChildren<TreeViewItem>();// Utils.FindVisualChildren<TreeViewItem>(vSourceItem);
            //    var visualTargetItem = dropInfo.VisualTargetItem as TreeViewItem;

            //    if (targetItem is AssetDirectory)
            //        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

            //    if (vSourceItem == visualTargetItem || vSourceChild.ToList().Contains(visualTargetItem) || visualTargetItem == null)
            //        dropInfo.Effects = DragDropEffects.None;
            //    else
            //        dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            //}
        }


        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;

            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                foreach (string str in dataObject.GetFileDropList())
                {

                    if (File.Exists(str))
                        CreateAssetFromPath(str);

                    if (Directory.Exists(str))
                        CreateAssetFromDirectory(new DirectoryInfo(str));
                }
            }

            //else if (dropInfo.DragInfo.Data is List<AssetDragDrop> && dropInfo.TargetCollection is ObservableCollection<IAsset>)
            //{
            //    var targetCollection = dropInfo.TargetCollection as ObservableCollection<IAsset>;
            //    if (targetCollection is ObservableCollection<IAsset>)
            //    {
            //        var targetItem = dropInfo.TargetItem;
            //        if (targetItem is AssetDirectory)
            //        {
            //            var data = dropInfo.DragInfo.Data as List<AssetDragDrop>;
            //            foreach (AssetDragDrop item in data)
            //            {
            //                item.DragObject.IsSelected = false;
            //                targetCollection.Add(item.DragObject);
            //                item.SourceCollection.Remove(item.DragObject);
            //            }
            //            ((AssetDirectory)targetItem).IsExpanded = true;
            //            ((AssetDirectory)targetItem).SortAssets();
            //        }
            //    }
            //}
        }

        public void RemoveAssets(List<IAsset> assetsToRemove, List<IAsset> assets)
        {
            assets.RemoveAll(item => assetsToRemove.Contains(item));
        }

        public void GetDragDropObjects(List<AssetDragDrop> dragList, ObservableCollection<IAsset> assets)
        {
            foreach (var asset in assets)
            {
                if (asset.IsSelected)
                {
                    AssetDragDrop dragDropObject = new AssetDragDrop();
                    dragDropObject.DragObject = asset;
                    dragDropObject.SourceCollection = assets;
                    dragList.Add(dragDropObject);
                }
                else if (!asset.IsSelected && asset is AssetDirectory)
                {
                    GetDragDropObjects(dragList, ((AssetDirectory)asset).Assets);
                }
            }
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            List<AssetDragDrop> dragList = new List<AssetDragDrop>();
            GetDragDropObjects(dragList, this.Assets);

            if (dragList.Any())
            {
                dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                dragInfo.Data = dragList;
            }
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is IAsset)
                return true;

            return false;
        }

        public void Dropped(IDropInfo dropInfo)
        {

        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {

        }

        public void DragCancelled()
        {

        }

        public bool TryCatchOccurredException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanRenameAsset = (SelectedItems.Count == 1 && SelectedItems.OfType<AssetDirectory>().Any());
            CanAddAsset = (SelectedItems.Count == 1 && SelectedItems.OfType<AssetDirectory>().Any());
            CanDeleteAsset = !SelectedItems.OfType<AssetDirectory>().Any(c => c.IsRoot == true);
            CanRelinkAsset = (SelectedItems.Count == 1 && !SelectedItems.OfType<AssetDirectory>().Any());

            if (SelectedItems.Count == 0)
            {
                CanAddAsset = false;
                CanRenameAsset = false;
                CanDeleteAsset = false;
                CanRelinkAsset = false;
            }
        }


    }
}



//private ObservableCollection<IAsset> _assetsFlatten;
//public ObservableCollection<IAsset> AssetsFlatten
//{
//    get => _assetsFlatten;
//    set => SetProperty(ref _assetsFlatten, value);
//}

//public CollectionViewSource GeometryViewSource { get; set; }
//private ICollectionView _geometryCollectionView;
//public ICollectionView GeometryCollectionView
//{
//    get => _geometryCollectionView;
//    set => SetProperty(ref _geometryCollectionView, value);
//}

//public CollectionViewSource ImageViewSource { get; set; }
//private ICollectionView _imageCollectionView;
//public ICollectionView ImageCollectionView
//{
//    get => _imageCollectionView;
//    set => SetProperty(ref _imageCollectionView, value);
//}

//public void InitCollectionView()
//{
//    GeometryViewSource = new CollectionViewSource();
//    GeometryViewSource.Source = this.AssetsFlatten;
//    GeometryCollectionView = GeometryViewSource.View;
//    GeometryCollectionView.SortDescriptions.Add(new SortDescription(nameof(AssetGeometry.Name), ListSortDirection.Ascending));
//    GeometryCollectionView.Filter = FilterGeometry;

//    ImageViewSource = new CollectionViewSource();
//    ImageViewSource.Source = this.AssetsFlatten;
//    ImageCollectionView = ImageViewSource.View;
//    ImageCollectionView.SortDescriptions.Add(new SortDescription(nameof(AssetTexture.Name), ListSortDirection.Ascending));
//    ImageCollectionView.Filter = FilterImage;
//}

//private void FlattenAssets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//{
//    Console.WriteLine("POUETPOUET");
//    //GeometryCollectionView.Refresh();
//    //ImageCollectionView.Refresh();
//}


//public void BuildAssetFlattenCollection(ObservableCollection<Asset> assets)
//{
//    foreach (Asset asset in assets)
//    {
//        if (asset is AssetDirectory)
//            BuildAssetFlattenCollection(((AssetDirectory)asset).Assets);
//        else
//            AssetsFlatten.Add(asset);
//    }
//}


//public bool FilterGeometry(object item) => item is AssetGeometry ? true : false;
//public bool FilterImage(object item) => item is AssetTexture ? true : false;

