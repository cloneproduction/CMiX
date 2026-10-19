using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // A list box of Avalonia clears its selection on a click with the Ctrl key.
    // The list and the model must not disagree afterwards.
    public class CtrlClickSelectionTests
    {
        [AvaloniaFact]
        public void ACtrlClickOnTheSelectedRow_LeavesTheListAndTheModelInAgreement()
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
            var list = view.GetVisualDescendants().OfType<ListBox>().First();
            var selectedRow = view.GetVisualDescendants().OfType<ComposableManagerItem>().ElementAt(2);
            var point = selectedRow.TranslatePoint(new Point(selectedRow.Bounds.Width / 2, selectedRow.Bounds.Height / 2), window)!.Value;

            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(point, MouseButton.Left, RawInputModifiers.Control);
            window.MouseUp(point, MouseButton.Left, RawInputModifiers.Control);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(layers.ManagerData.SelectedIndex, list.SelectedIndex);
        }
    }
}
