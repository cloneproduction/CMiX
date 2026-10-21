using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // A right click on a row selects it and opens its menu, as in the outliner of Blender.
    public class RowRightClickTests
    {
        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public PrefabManager Layers { get; init; } = null!;
            public ListBox List { get; init; } = null!;
            public List<ComposableManagerItem> Rows { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;

            public int UndoSteps =>
                ((System.Collections.ICollection)typeof(UndoManager)
                    .GetField("_undoStack", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(Undo)!).Count;

            public bool AMenuIsOpen => Rows.Any(row => row.ContextMenu?.IsOpen == true);

            public Point CenterOf(Control control) =>
                control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), Window)!.Value;

            public void RightDown(int row)
            {
                var point = CenterOf(Rows[row]);
                Window.MouseMove(point);
                Dispatcher.UIThread.RunJobs();
                Window.MouseDown(point, MouseButton.Right);
                Dispatcher.UIThread.RunJobs();
            }

            public void RightUp(int row)
            {
                Window.MouseUp(CenterOf(Rows[row]), MouseButton.Right);
                Dispatcher.UIThread.RunJobs();
            }
        }

        private static Screen Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));

            var view = new ComposableManager { ItemType = typeof(Layer), DataContext = layers };
            var window = new Window { Content = view, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Screen
            {
                Window = window,
                Layers = layers,
                List = view.GetVisualDescendants().OfType<ListBox>().First(),
                Rows = view.GetVisualDescendants().OfType<ComposableManagerItem>().ToList(),
                Undo = undo
            };
        }

        [AvaloniaFact]
        public void RightClickOnAnUnselectedRow_SelectsItWhenThePressStarts_AndOpensTheMenuWhenItEnds()
        {
            var screen = Show();
            Assert.Equal(2, screen.Layers.ManagerData.SelectedIndex);

            screen.RightDown(0);

            Assert.Equal(0, screen.Layers.ManagerData.SelectedIndex);
            Assert.Equal(0, screen.List.SelectedIndex);
            Assert.False(screen.AMenuIsOpen);

            screen.RightUp(0);

            Assert.True(screen.Rows[0].ContextMenu!.IsOpen);
            Assert.Equal(0, screen.Layers.ManagerData.SelectedIndex);
        }

        [AvaloniaFact]
        public void RightClickOnAnUnselectedRow_IsOneUndoStepLikeALeftClick()
        {
            var screen = Show();

            screen.RightDown(0);
            screen.RightUp(0);

            Assert.Equal(1, screen.UndoSteps);
        }

        [AvaloniaFact]
        public void RightClickOnTheSelectedRow_KeepsTheSelectionAndAddsNoUndoStep()
        {
            var screen = Show();

            screen.RightDown(2);
            screen.RightUp(2);

            Assert.Equal(2, screen.Layers.ManagerData.SelectedIndex);
            Assert.Equal(0, screen.UndoSteps);
            Assert.True(screen.Rows[2].ContextMenu!.IsOpen);
        }
    }
}
