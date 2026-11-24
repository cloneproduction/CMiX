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
        public void LeftPanel_ByDefault_ShowsOneListTreeProjectOfTheProjectTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();

            Assert.Equal(PanelChoice.Tree, viewModel.LeftPanel);
            var view = Assert.Single(window.GetVisualDescendants().OfType<ListTreeProject>());
            Assert.Same(viewModel.ProjectTree, view.DataContext);
        }

        [AvaloniaFact]
        public void RightPanel_TheTree_ShowsTwoListTreeProjectsOfOneProjectTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.RightPanel = PanelChoice.Tree;
            TestServiceProviderFactory.Pump();

            var views = window.GetVisualDescendants().OfType<ListTreeProject>().ToList();
            Assert.Equal(2, views.Count);
            Assert.All(views, view => Assert.Same(viewModel.ProjectTree, view.DataContext));
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
        public void IsActiveView_TwoViewsWithNoFocusAndNoPointer_TheFirstAttachedViewIsActive()
        {
            var scene = CreateScene();
            var first = new ListTreeProject { DataContext = scene.Tree };
            var second = new ListTreeProject { DataContext = scene.Tree };
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            Grid.SetColumn(second, 1);
            grid.Children.Add(first);
            grid.Children.Add(second);

            Show(grid);

            Assert.True(first.IsActiveView);
            Assert.False(second.IsActiveView);
        }
    }
}
