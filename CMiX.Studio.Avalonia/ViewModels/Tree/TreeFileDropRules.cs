// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Session;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed class TreeFileDropRules
    {
        private readonly PathDropRules pathDrop;
        private readonly PrefabManager compositions;
        private readonly UndoManager undoManager;

        public TreeFileDropRules(PathDropRules pathDrop, PrefabManager compositions, UndoManager undoManager)
        {
            this.pathDrop = pathDrop;
            this.compositions = compositions;
            this.undoManager = undoManager;
        }

        // No node, or a node under no composition, gives null: the drop then makes a new composition.
        public TreeNode? FindTarget(TreeNode? hovered) =>
            hovered == null ? null : TreeWalk.ParentsOf(hovered).Prepend(hovered).FirstOrDefault(row => row.Item is Composition or Layer);

        // The other nodes of a layer pass the drop to its entities but are not the target.
        public bool IsTarget(TreeNode node) =>
            node.Item is Composition or Layer
            || node.Group is EntitiesGroup;

        public bool Accepts(string path) => pathDrop.AcceptsItem(path);

        public bool CanDrop(IEnumerable<string> paths) => paths.Any(pathDrop.AcceptsItem);

        public void Drop(TreeNode? hovered, IReadOnlyList<string> paths)
        {
            var accepted = paths.Where(pathDrop.AcceptsItem).ToList();
            if (accepted.Count == 0)
                return;

            var target = FindTarget(hovered);
            using (undoManager.Capture())
            {
                switch (target?.Item)
                {
                    case Layer layer:
                        AddPaths(layer, accepted);
                        break;
                    case Composition composition:
                        AddLayerWithPaths(composition, accepted);
                        break;
                    default:
                        compositions.AddItem(typeof(Composition), item => AddLayerWithPaths((Composition)item, accepted));
                        break;
                }
            }
        }

        private void AddLayerWithPaths(Composition composition, IReadOnlyList<string> paths) =>
            composition.LayerManager.AddItem(typeof(Layer), item => AddPaths((Layer)item, paths));

        private void AddPaths(Layer layer, IEnumerable<string> paths)
        {
            foreach (var path in paths)
                pathDrop.AddItem(layer.ModelEntityManager, path);
        }
    }
}
