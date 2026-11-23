// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The main window with one composition of three empty layers, all rows open. A list tree moves its rows with no animation.
    internal record ListTreeMainScreen(IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel)
    {
        public ProjectTree Tree => ViewModel.ProjectTree;
        public ListTreeProject View => Window.GetVisualDescendants().OfType<ListTreeProject>().Single();
        public Composition Composition => (Composition)ViewModel.Project.CompositionManager.ManagerData.Items[0];
        public TreeNode CompositionNode => Tree.CompositionNodes()[0];
        public TreeNode LayerNode(int index) => CompositionNode.Children[0].Children[index];
        public Layer Layer(int index) => (Layer)Composition.LayerManager.ManagerData.Items[index];

        public static ListTreeMainScreen ShowWithThreeLayers(PanelChoice panel)
        {
            var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.LeftPanel = panel;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 3; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            foreach (var view in window.GetVisualDescendants().OfType<ListTreeProject>())
                view.Rows.MoveDuration = TimeSpan.Zero;
            return new ListTreeMainScreen(provider, window, viewModel);
        }
    }
}
