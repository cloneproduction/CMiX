// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.IO;
using System.Linq;
using System.Windows;
using CMiX.Core.BaseControls;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Studio.Services
{
    public class AssetSelectorDropHandler : IDropTarget
    {
        private readonly AssetSelector _assetSelector;

        public AssetSelectorDropHandler(AssetSelector assetSelector)
        {
            _assetSelector = assetSelector;
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

            var assets = dataObject.GetFileDropList()
                                   .Cast<string>()
                                   .Where(File.Exists)
                                   .Select(_assetSelector.CreateAssetFromPath)
                                   .Where(a => a != null)
                                   .ToList();

            foreach (var asset in assets)
                _assetSelector.AssetRepository.Add(asset);

            var first = assets.FirstOrDefault();
            if (first == null) return;

            _assetSelector.Asset = first;
            _assetSelector.FilePath.Value = first.FilePath;
        }

        public void DragEnter(IDropInfo dropInfo) { }
        public void DragLeave(IDropInfo dropInfo) { }
    }
}
