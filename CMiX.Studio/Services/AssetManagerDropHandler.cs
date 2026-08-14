// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using CMiX.Core.Assets;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Studio.Services
{
    public class AssetManagerDropHandler : IDropTarget, IDragSource
    {
        private readonly AssetManager _assetManager;

        public AssetManagerDropHandler(AssetManager assetManager)
        {
            _assetManager = assetManager;
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
                if (File.Exists(str)) _assetManager.CreateAssetFromPath(str);
                if (Directory.Exists(str)) _assetManager.CreateAssetFromDirectory(new DirectoryInfo(str));
            }
        }

        public void StartDrag(IDragInfo dragInfo) { }
        public bool CanStartDrag(IDragInfo dragInfo) => dragInfo.SourceItem is IAsset;
        public void Dropped(IDropInfo dropInfo) { }
        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo) { }
        public void DragCancelled() { }
        public bool TryCatchOccurredException(Exception exception) => throw new NotImplementedException();
        public void DragEnter(IDropInfo dropInfo) { }
        public void DragLeave(IDropInfo dropInfo) { }
    }
}
