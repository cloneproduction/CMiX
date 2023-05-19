// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.Collections;
using CMiX.Core.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.ViewModels.Assets
{
    public class AssetManager : ObservableRecipient, IDropTarget, IDragSource
    {
        public AssetManager(IProject project)
        {
            Project = project;

            VideoAssets = new SortableObservableCollection<Asset>();
            ImageAssets = new SortableObservableCollection<Asset>();
            GeometryAssets = new SortableObservableCollection<Asset>();


            SelectedItems = new ObservableCollection<IAsset>();
            SelectedItems.CollectionChanged += CollectionChanged;

            AddAssetCommand = new RelayCommand(AddAsset);
            DeleteAssetsCommand = new RelayCommand(DeleteAssets);
            RenameAssetCommand = new RelayCommand(RenameAsset);
            RelinkAssetsCommand = new RelayCommand(RelinkAssets);
        }

        public IProject Project { get; set; }


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


        public SortableObservableCollection<Asset> VideoAssets { get; set; }
        public SortableObservableCollection<Asset> ImageAssets { get; set; }
        public SortableObservableCollection<Asset> GeometryAssets { get; set; }


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
            //if (SelectedItems.Take(2).Count() == 1)
            //{
            //    IAsset asset = SelectedItems.First();
            //    OpenFileDialogSettings settings = new OpenFileDialogSettings();

            //    if (asset is AssetImage)
            //        settings.Filter = "Image |*.jpg;*.jpeg;*.png;*.dds";

            //    if (asset is AssetGeometry)
            //        settings.Filter = "Geometry |*.fbx; *.obj";

            //    if (asset is AssetVideo)
            //        settings.Filter = "Video |*.mov";

            //    bool? success = DialogService.ShowOpenFileDialog(this, settings);
            //    if (success == true)
            //    {
            //        asset.Path = settings.FileName;
            //        asset.Name = Path.GetFileName(settings.FileName);
            //    }
            //}
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
            //DeleteSelectedAssets(this.Assets);
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
                //this.Assets.Remove(item);
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
                string fileType = Path.GetExtension(path).ToUpper().TrimStart('.');

                if(fileType == "PNG" || fileType == "JPG" || fileType == "JPEG")
                {
                    ImageAssets.Add(new Asset(path));
                    return;
                }

                if(fileType == "OBJ")
                {
                    GeometryAssets.Add(new Asset(path));
                    return;
                }

                if(fileType == "MOV")
                {
                    VideoAssets.Add(new Asset(path));
                    return;
                }
            }
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
            //List<AssetDragDrop> dragList = new List<AssetDragDrop>();
            //GetDragDropObjects(dragList, this.Assets);

            //if (dragList.Any())
            //{
            //    dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            //    dragInfo.Data = dragList;
            //}
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

        public void DragEnter(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }
    }
}
