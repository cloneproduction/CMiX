// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using CMiX.UI.Controls.Path;

namespace CMiX.Studio.Avalonia.Views
{
    // The PathSelector of the Studio. The DataContext is an AssetSelector: a path the user
    // chooses or drops becomes an asset of its repository.
    public class AssetPathSelector : PathSelector
    {
        public AssetPathSelector()
        {
            Extensions = AssetTypes.AllExtensions;
            FileTypeName = "Assets";
        }

        protected override void OnPathChosen(string path)
        {
            base.OnPathChosen(path);

            if (DataContext is AssetSelector assetSelector)
                assetSelector.SetAssetFromPath(path);
        }

        protected override void OnPathsDropped(IReadOnlyList<string> paths)
        {
            base.OnPathsDropped(paths);

            if (DataContext is not AssetSelector assetSelector)
                return;

            var assets = paths
                .Select(assetSelector.CreateAssetFromPath)
                .Where(asset => asset != null)
                .ToList();

            foreach (var asset in assets)
                assetSelector.AssetRepository.Add(asset);

            var first = assets.FirstOrDefault();
            if (first == null)
                return;

            assetSelector.Asset = first;
            assetSelector.FilePath.Value = first.FilePath;
        }
    }
}
