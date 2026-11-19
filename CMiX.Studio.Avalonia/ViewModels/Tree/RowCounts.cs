// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // A recount that changes a count recounts the row above too.
    internal sealed class RowCounts
    {
        private readonly TreeNode row;
        private readonly RowCounts? parentCounts;
        private IReadOnlyList<KindCount> kindCounts = Array.Empty<KindCount>();
        private int missingCount;
        private int hiddenCount;

        public RowCounts(TreeNode row, RowCounts? parentCounts)
        {
            this.row = row;
            this.parentCounts = parentCounts;
        }

        public IReadOnlyList<KindCount> KindCounts => kindCounts;
        public int KindCountsComputations { get; private set; }
        public bool HasKindCounts => kindCounts.Count > 0;

        public IReadOnlyList<KindCount> ShownKindCounts =>
            kindCounts.Count <= TreeNode.MaxShownKindCounts ? kindCounts : kindCounts.Take(TreeNode.MaxShownKindCounts).ToList();

        public int MoreKindsCount => Math.Max(0, kindCounts.Count - TreeNode.MaxShownKindCounts);

        public string MoreKindsDescription =>
            string.Join(Environment.NewLine, kindCounts.Skip(TreeNode.MaxShownKindCounts).Select(count => count.Description));

        // A path with no asset counts as missing too.
        public bool IsMissingAsset =>
            row.Item is IAssetTextureSource { AssetSelector: { } selector }
            && (selector.Asset is { FileExist: false } || (selector.Asset == null && !string.IsNullOrEmpty(selector.FilePath?.Value)));

        public int MissingCount => missingCount;
        public bool HasMissingAssets => missingCount > 0;
        public int HiddenCount => hiddenCount;
        public bool HasHiddenRows => hiddenCount > 0;

        public void Count()
        {
            kindCounts = ComputeKindCounts();
            hiddenCount = ComputeHiddenCount();
            missingCount = ComputeMissingCount();
        }

        public void WatchAsset()
        {
            if (row.Item is IAssetTextureSource { AssetSelector: { } selector })
                selector.PropertyChanged += OnAssetSelectorChanged;
        }

        public void UnwatchAsset()
        {
            if (row.Item is IAssetTextureSource { AssetSelector: { } selector })
                selector.PropertyChanged -= OnAssetSelectorChanged;
        }

        public void RecountAll()
        {
            RecountKinds();
            RecountHidden();
            RecountMissing();
        }

        public void RecountHiddenAbove() => parentCounts?.RecountHidden();

        private List<KindCount> ComputeKindCounts()
        {
            KindCountsComputations++;
            var counts = new Dictionary<TreeCategory, int>();
            foreach (var child in row.Children)
            {
                if (child.Category is not { } category)
                    continue;

                var number = child.Item != null ? 1 : child.Children.Count;
                if (number > 0)
                    counts[category] = counts.GetValueOrDefault(category) + number;
            }

            return counts.Select(count => new KindCount(count.Key, count.Value)).ToList();
        }

        private int ComputeMissingCount() => (IsMissingAsset ? 1 : 0) + row.Children.Sum(child => child.MissingCount);

        private int ComputeHiddenCount() => row.Children.Sum(child => (child.IsHidden ? 1 : 0) + child.HiddenCount);

        // The rows under a group change what its parent holds.
        private void RecountKinds()
        {
            kindCounts = ComputeKindCounts();
            row.RaiseKindCountsChanged();
            if (row.Item == null)
                parentCounts?.RecountKinds();
        }

        private void RecountMissing()
        {
            var count = ComputeMissingCount();
            if (count == missingCount)
                return;

            missingCount = count;
            row.RaiseMissingCountChanged();
            parentCounts?.RecountMissing();
        }

        private void RecountHidden()
        {
            var count = ComputeHiddenCount();
            if (count == hiddenCount)
                return;

            var had = HasHiddenRows;
            hiddenCount = count;
            row.RaiseHiddenCountChanged();
            if (had != HasHiddenRows)
                row.RaiseHasHiddenRowsChanged();

            parentCounts?.RecountHidden();
        }

        private void OnAssetSelectorChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AssetSelector.Asset))
                RecountMissing();
        }
    }
}
