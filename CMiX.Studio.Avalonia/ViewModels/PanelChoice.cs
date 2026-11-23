// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels
{
    // What a side panel of the main window can show. The user picks it in the list that opens from the icon at the top of the panel.
    // Each choice is its own type, so the panel picks the view with a data template.
    // To add a choice: add a type here, add it to All, and add a data template for it in PanelSide.
    public abstract class PanelChoice
    {
        protected PanelChoice(string name, string iconKey)
        {
            Name = name;
            IconKey = iconKey;
        }

        // The text of the choice in the list. The panel shows only the icon.
        public string Name { get; }

        // The key of the icon of the choice.
        public string IconKey { get; }

        public static PanelChoice Tree { get; } = new TreePanelChoice();
        public static PanelChoice ListTree { get; } = new ListTreePanelChoice();
        public static PanelChoice Tabs { get; } = new TabsPanelChoice();
        public static PanelChoice Assets { get; } = new AssetsPanelChoice();

        public static IReadOnlyList<PanelChoice> All { get; } = new[] { Tree, ListTree, Tabs, Assets };

        public override string ToString() => Name;
    }

    // The project tree.
    public sealed class TreePanelChoice : PanelChoice
    {
        public TreePanelChoice() : base("Tree", "Tree") { }
    }

    // The new project tree: one flat list of rows. It shows the same project tree as the choice Tree.
    public sealed class ListTreePanelChoice : PanelChoice
    {
        public ListTreePanelChoice() : base("List tree", "Tree") { }
    }

    // The tabs of the app, with the master beat under them.
    public sealed class TabsPanelChoice : PanelChoice
    {
        public TabsPanelChoice() : base("Tabs", "Tabs") { }
    }

    // The assets of the project: the folders and files that the items point to.
    public sealed class AssetsPanelChoice : PanelChoice
    {
        public AssetsPanelChoice() : base("Assets", "Resources") { }
    }
}
