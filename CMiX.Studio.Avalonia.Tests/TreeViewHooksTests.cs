using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // Two views can show the same tree (both side panels). Each view gives its own hooks. A view that goes takes back only its own.
    public class TreeViewHooksTests
    {
        // The order of the active view: the focus in the rows, then the focus in the panel, then the pointer, then the first view.
        [Fact]
        public void IsActiveView_FocusInRowsPanelAndPointer_FollowsTheOrder()
        {
            var tree = CreateScene().Tree;
            var rows = false;
            var panel = false;
            var pointer = new TreeViewHooks { IsPointerOverTree = () => true };
            var focus = new TreeViewHooks { IsTreeFocused = () => rows, IsPanelFocused = () => panel };
            tree.AttachView(pointer);
            tree.AttachView(focus);

            Assert.True(tree.IsActiveView(pointer));

            panel = true;
            Assert.True(tree.IsActiveView(focus));

            panel = false;
            rows = true;
            Assert.True(tree.IsActiveView(focus));
        }

        [Fact]
        public void IsActiveView_RowsFocusedInOneViewAndPanelFocusedInTheOther_TheRowsWin()
        {
            var tree = CreateScene().Tree;
            var panel = new TreeViewHooks { IsPanelFocused = () => true };
            var rows = new TreeViewHooks { IsTreeFocused = () => true };
            tree.AttachView(panel);
            tree.AttachView(rows);

            Assert.True(tree.IsActiveView(rows));
            Assert.False(tree.IsActiveView(panel));
        }

        [Fact]
        public void DetachView_OneOfTwoViews_TheOtherViewStillAnswers()
        {
            var tree = CreateScene().Tree;
            var toggled = 0;
            var cancelled = 0;
            var first = new TreeViewHooks
            {
                IsPointerOverTree = () => true,
                IsTreeFocused = () => true,
                IsTextBoxFocused = () => true,
                ToggleDragRow = () => toggled++,
                CancelDrag = () => cancelled++
            };
            var second = new TreeViewHooks();
            tree.AttachView(first);
            tree.AttachView(second);

            tree.DetachView(second);

            Assert.True(tree.Views.IsPointerOverTree());
            Assert.True(tree.Views.IsTreeFocused());
            Assert.True(tree.Views.IsTextBoxFocused());
            tree.Views.ToggleDragRow();
            tree.Views.CancelDrag();
            Assert.Equal(1, toggled);
            Assert.Equal(1, cancelled);
        }

        [Fact]
        public void AttachView_SecondView_DoesNotHideTheFirstView()
        {
            var tree = CreateScene().Tree;
            tree.AttachView(new TreeViewHooks { IsPointerOverTree = () => true });

            tree.AttachView(new TreeViewHooks());

            Assert.True(tree.Views.IsPointerOverTree());
        }

        // The focus in the panel of one view is not in another panel, so every view must say that the focus is elsewhere.
        [Fact]
        public void IsFocusInOtherPanel_TwoViews_IsTrueOnlyWhenEveryViewSaysSo()
        {
            var tree = CreateScene().Tree;
            var focusInSecond = true;
            Assert.False(tree.Views.IsFocusInOtherPanel());
            tree.AttachView(new TreeViewHooks { IsFocusInOtherPanel = () => true });
            tree.AttachView(new TreeViewHooks { IsFocusInOtherPanel = () => !focusInSecond });

            Assert.False(tree.Views.IsFocusInOtherPanel());

            focusInSecond = false;
            Assert.True(tree.Views.IsFocusInOtherPanel());
        }

        [Fact]
        public void DetachView_LastView_TheHooksAnswerNo()
        {
            var tree = CreateScene().Tree;
            var view = new TreeViewHooks { IsPointerOverTree = () => true, IsTreeFocused = () => true };
            tree.AttachView(view);

            tree.DetachView(view);

            Assert.False(tree.Views.IsPointerOverTree());
            Assert.False(tree.Views.IsTreeFocused());
        }

        [Fact]
        public void CloseRowsInView_TwoViews_GoesToTheFocusedViewOnce()
        {
            var tree = CreateScene().Tree;
            var asked = new List<string>();
            var closes = 0;
            tree.AttachView(new TreeViewHooks { CloseRowsInView = (_, close) => { asked.Add("first"); close(); } });
            tree.AttachView(new TreeViewHooks { IsTreeFocused = () => true, CloseRowsInView = (_, close) => { asked.Add("second"); close(); } });

            tree.Views.CloseRowsInView(null, () => { closes++; return Array.Empty<TreeNode>(); });

            Assert.Equal(new[] { "second" }, asked);
            Assert.Equal(1, closes);
        }

        [AvaloniaFact]
        public void Unwatch_SecondViewLeaves_TheFocusedViewStillTellsTheTreeHasTheFocus()
        {
            var scene = CreateScene();
            var first = new TreeViewProject { DataContext = scene.Tree };
            var second = new TreeViewProject { DataContext = scene.Tree };
            var panel = new StackPanel { Children = { first, second } };
            var window = new Window { Content = panel, Width = 400, Height = 1100 };
            window.Show();
            TreeTestScreen.Settle(window);
            try
            {
                Assert.True(TreeTestScreen.ItemOf(first, scene.Tree.Roots[0]).Focus(NavigationMethod.Tab));
                Dispatcher.UIThread.RunJobs();
                Assert.True(scene.Tree.Views.IsTreeFocused());

                panel.Children.Remove(second);
                Dispatcher.UIThread.RunJobs();

                Assert.True(scene.Tree.Views.IsTreeFocused());
            }
            finally
            {
                window.Close();
            }
        }
    }
}
