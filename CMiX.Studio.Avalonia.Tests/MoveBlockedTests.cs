using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A move that shared items block is explained to the user in a dialog of the style of the other dialogs.
    public class MoveBlockedTests
    {
        private static (ProjectTree Tree, FakeMoveBlockedNotice Notice, Composition A, Composition B) CreateScene()
        {
            var scene = TreeFixtures.CreateScene();
            return (scene.Tree, scene.Provider.GetRequiredService<FakeMoveBlockedNotice>(), scene.A, scene.B);
        }

        // The first layer of A shares an entity with the second.
        private static void ShareAnEntity(Composition a)
        {
            var first = (Layer)a.LayerManager.ManagerData.Items[0];
            first.ModelEntityManager.AddItem(typeof(Entity));
            ((Layer)a.LayerManager.ManagerData.Items[1]).ModelEntityManager.AddExistingItem((IPrefab)first.ModelEntityManager.ManagerData.Items[0]);
        }

        [Fact]
        public async Task DropAsync_ABlockedMove_TellsTheUserWhy()
        {
            var (tree, notice, a, _) = CreateScene();
            ShareAnEntity(a);

            await tree.DropAsync(tree.CompositionNodes()[0].Children[0].Children[0], tree.CompositionNodes()[1], DropPosition.Inside);

            var message = Assert.Single(notice.Messages);
            Assert.Contains(a.PrefabService.Name.Value, message);
            Assert.Equal(2, a.LayerManager.ManagerData.Items.Count);
        }

        [Fact]
        public async Task DropAsync_AMoveThatWorks_TellsNothing()
        {
            var (tree, notice, a, b) = CreateScene();

            await tree.DropAsync(tree.CompositionNodes()[0].Children[0].Children[0], tree.CompositionNodes()[1], DropPosition.Inside);

            Assert.Empty(notice.Messages);
            Assert.Single(a.LayerManager.ManagerData.Items);
            Assert.Equal(2, b.LayerManager.ManagerData.Items.Count);
        }

        [Fact]
        public void CanDrop_AnyMove_AsksTheRules()
        {
            var (tree, _, _, _) = CreateScene();

            Assert.True(tree.CanDrop(tree.CompositionNodes()[0], tree.CompositionNodes()[1], DropPosition.After));
            Assert.False(tree.CanDrop(tree.CompositionNodes()[0], tree.CompositionNodes()[1], DropPosition.Inside));
        }

        [AvaloniaFact]
        public void Show_TheDialog_ShowsTheMessage()
        {
            var dialog = new DialogMessage(MoveBlockedNotice.Options("The layer shares items."));
            dialog.Show();
            Dispatcher.UIThread.RunJobs();

            var texts = dialog.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text);

            Assert.Contains("The layer shares items.", texts);
        }

        [AvaloniaFact]
        public void Click_TheOkButton_ClosesTheDialog()
        {
            var dialog = new DialogMessage(MoveBlockedNotice.Options("The layer shares items."));
            var closed = false;
            dialog.Closed += (_, _) => closed = true;
            dialog.Show();
            Dispatcher.UIThread.RunJobs();

            dialog.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "OkButton")
                .RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            Assert.True(closed);
        }

        [AvaloniaFact]
        public void Press_Escape_ClosesTheDialog()
        {
            var dialog = new DialogMessage(MoveBlockedNotice.Options("The layer shares items."));
            var closed = false;
            dialog.Closed += (_, _) => closed = true;
            dialog.Show();
            Dispatcher.UIThread.RunJobs();

            dialog.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
            Dispatcher.UIThread.RunJobs();

            Assert.True(closed);
        }
    }
}
