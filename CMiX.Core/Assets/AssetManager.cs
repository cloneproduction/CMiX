// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows;
using CMiX.Core.Assets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Assets
{
    public partial class AssetManager : ObservableRecipient, IControl, IDropTarget, IDragSource
    {
        public AssetManager(AssetRepository assetRepository)
        {
            AssetRepository = assetRepository;
        }

        public Guid ID { get; set; }
        public AssetRepository AssetRepository { get; set; }
        public ObservableCollection<IAsset> SelectedItems { get; set; } = new();

        public void RenameAsset()
        {
            if (SelectedItems.Take(2).Count() == 1 && SelectedItems.First() is AssetDirectory assetDirectory)
                assetDirectory.Rename();
        }

        [RelayCommand]
        public void AddAsset()
        {
            if (SelectedItems.Take(2).Count() == 1 && SelectedItems.FirstOrDefault() is AssetDirectory assetDirectory)
            {
                assetDirectory.IsExpanded = true;
                assetDirectory.AddAsset(new AssetDirectory("New Folder"));
            }
        }

        [RelayCommand]
        private void DeleteAssets() { }

        public void RemoveItemFromDirectory(AssetDirectory directory)
        {
            var toBeRemoved = new List<IAsset>();
            foreach (var asset in directory.Assets)
            {
                if (asset is AssetDirectory subDir)
                    RemoveItemFromDirectory(subDir);
                toBeRemoved.Add(asset);
            }
            foreach (var item in toBeRemoved)
                directory.Assets.Remove(item);
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
                assets.Remove(item);
            SelectedItems.Clear();
        }

        private void CreateAssetFromDirectory(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
                CreateAssetFromDirectory(directory);
            foreach (var file in directoryInfo.GetFiles())
                CreateAssetFromPath(file.FullName);
        }

        private void CreateAssetFromPath(string path)
        {
            if (!File.Exists(path)) return;
            string fileType = Path.GetExtension(path).ToUpper().TrimStart('.');

            if (fileType is "PNG" or "JPG" or "JPEG")
                AssetRepository.Add(new ImageAsset(path));
            else if (fileType is "OBJ" or "FBX")
                AssetRepository.Add(new Geometry(path));
            else if (fileType == "MOV")
                AssetRepository.Add(new Video(path));
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject dataObject && dataObject.ContainsFileDropList())
                dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not DataObject dataObject || !dataObject.ContainsFileDropList())
                return;

            foreach (string str in dataObject.GetFileDropList())
            {
                if (File.Exists(str)) CreateAssetFromPath(str);
                if (Directory.Exists(str)) CreateAssetFromDirectory(new DirectoryInfo(str));
            }
        }

        public void RemoveAssets(List<IAsset> assetsToRemove, List<IAsset> assets) =>
            assets.RemoveAll(item => assetsToRemove.Contains(item));

        public void StartDrag(IDragInfo dragInfo) { }
        public bool CanStartDrag(IDragInfo dragInfo) => dragInfo.SourceItem is IAsset;
        public void Dropped(IDropInfo dropInfo) { }
        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo) { }
        public void DragCancelled() { }
        public bool TryCatchOccurredException(Exception exception) => throw new NotImplementedException();
        public void DragEnter(IDropInfo dropInfo) { }
        public void DragLeave(IDropInfo dropInfo) { }

        public IControlModel ToModel() => new AssetManagerModel { ID = ID };
        public void FromModel(IControlModel model) => ID = ((AssetManagerModel)model).ID;
    }
}
