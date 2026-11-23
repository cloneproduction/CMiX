// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeProjectTests
    {
        private static Window Show(Control content)
        {
            var window = new Window { Content = content, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void LeftPanel_TheListTree_ShowsAListTreeProjectOfTheProjectTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.LeftPanel = PanelChoice.ListTree;
            TestServiceProviderFactory.Pump();

            var view = Assert.Single(window.GetVisualDescendants().OfType<ListTreeProject>());
            Assert.Same(viewModel.ProjectTree, view.DataContext);
            Assert.Empty(window.GetVisualDescendants().OfType<TreeViewProject>());
        }

        [AvaloniaFact]
        public void RightPanel_TheListTreeBesideTheTree_ShowsBothViewsOfOneProjectTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.RightPanel = PanelChoice.ListTree;
            TestServiceProviderFactory.Pump();

            var listTree = Assert.Single(window.GetVisualDescendants().OfType<ListTreeProject>());
            var tree = Assert.Single(window.GetVisualDescendants().OfType<TreeViewProject>());
            Assert.Same(tree.DataContext, listTree.DataContext);
        }

        [AvaloniaFact]
        public void OnAttachedToVisualTree_TheOnlyView_IsTheActiveViewOfTheTree()
        {
            var scene = CreateScene();
            var view = new ListTreeProject { DataContext = scene.Tree };

            Show(view);

            Assert.True(view.IsActiveView);
            Assert.NotNull(scene.Tree.Views.ActiveView());
        }

        [AvaloniaFact]
        public void OnDetachedFromVisualTree_TheOnlyView_LeavesTheTreeWithNoView()
        {
            var scene = CreateScene();
            var view = new ListTreeProject { DataContext = scene.Tree };
            var window = Show(view);

            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            Assert.False(view.IsActiveView);
            Assert.Null(scene.Tree.Views.ActiveView());
        }

        [AvaloniaFact]
        public void IsActiveView_BesideTheOldViewWithNoFocusAndNoPointer_TheFirstAttachedViewIsActive()
        {
            var scene = CreateScene();
            var tree = new TreeViewProject { DataContext = scene.Tree };
            var listTree = new ListTreeProject { DataContext = scene.Tree };
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            Grid.SetColumn(listTree, 1);
            grid.Children.Add(tree);
            grid.Children.Add(listTree);

            Show(grid);

            Assert.True(tree.IsActiveView);
            Assert.False(listTree.IsActiveView);
        }
    }
}
