using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Vector;
using CMiX.UI.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Use Composition Resolution toggle and the Resolution that every texture source shows.
    public class TextureResolutionTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        [AvaloniaFact]
        public void Toggle_UseCompositionResolution_DisablesTheResolution()
        {
            var source = (CMiX.Core.Texturing.Sources.Image)TestServiceProviderFactory.Create()
                .GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Sources.Image));
            var view = new TextureResolution { DataContext = source };
            var window = windows.Add(new Window { Content = view, Width = 500, Height = 300 });
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var toggle = view.GetVisualDescendants().OfType<ToggleButtonCMiX>().Single();
            var resolution = view.GetVisualDescendants().OfType<Integer2>().Single();
            Assert.False(source.UseCompositionResolution.Value);
            Assert.True(resolution.IsEffectivelyEnabled);

            PointerInput.Click(window, toggle, MouseButton.Left);
            TestServiceProviderFactory.Pump();

            Assert.True(source.UseCompositionResolution.Value);
            Assert.False(resolution.IsEffectivelyEnabled);
        }
    }
}
