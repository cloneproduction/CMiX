// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Services;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed class TreeFileDropRules
    {
        private readonly PathDropService pathDrop;
        private readonly PrefabManager compositions;
        private readonly UndoManager undoManager;

        public TreeFileDropRules(PathDropService pathDrop, PrefabManager compositions, UndoManager undoManager)
        {
            this.pathDrop = pathDrop;
            this.compositions = compositions;
            this.undoManager = undoManager;
        }

        // The node that takes dropped files: the first composition or layer from the node up. No node, or a node
        // that is under no composition, gives null, and then the drop makes a new composition.
        public TreeNode? FindTarget(TreeNode? hovered) =>
            hovered == null ? null : TreeWalk.ParentsOf(hovered).Prepend(hovered).FirstOrDefault(row => row.Item is Composition or Layer);

        // True when the node itself takes the dropped files: a composition, a layer, or the group of entities of a layer.
        // The other nodes of a layer pass the drop to the entities of their layer, but they are not the target themselves.
        public bool IsTarget(TreeNode node) =>
            node.Item is Composition or Layer
            || node.Group is EntitiesGroup;

        public bool Accepts(string path) => pathDrop.AcceptsItem(path);

        public bool CanDrop(IEnumerable<string> paths) => paths.Any(pathDrop.AcceptsItem);

        // Makes what the dropped paths need, and adds an item for each path, as one undo step.
        // A composition takes a new layer, and a layer takes the items. With no target, a new composition with a new
        // layer takes them. A drop of nothing that is accepted changes nothing.
        public void Drop(TreeNode? hovered, IReadOnlyList<string> paths)
        {
            var accepted = paths.Where(pathDrop.AcceptsItem).ToList();
            if (accepted.Count == 0)
                return;

            var target = FindTarget(hovered);
            undoManager.BeginCapture();
            try
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
            finally
            {
                undoManager.EndCapture();
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
