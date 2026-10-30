using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The engine applies the transform of the diffuse texture, so the Transform Texture panel of the quad shows that one.
    public class FullscreenQuadViewTests
    {
        private static (FullscreenQuad Quad, ExpanderIndented Expander) ShowTransformTexture()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var quad = (FullscreenQuad)factory.Create(typeof(FullscreenQuad));
            var view = new Views.FullscreenQuad { DataContext = quad };
            new Window { Content = new ScrollViewer { Content = view }, Width = 400, Height = 1200 }.Show();
            Dispatcher.UIThread.RunJobs();

            var expander = view.GetVisualDescendants().OfType<ExpanderIndented>()
                .First(expander => Equals(expander.Header, "Transform Texture"));
            expander.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            return (quad, expander);
        }

        [AvaloniaFact]
        public void TransformTexturePanel_ShowsTheTransformOfTheDiffuseTexture()
        {
            var (quad, expander) = ShowTransformTexture();

            var transform = expander.GetVisualDescendants().OfType<Views.Transform2D>().Single();

            Assert.Same(quad.Texture.DiffuseTexture.Transform2D, transform.DataContext);
        }

        [AvaloniaFact]
        public void TransformTexturePanel_ShowsTheSamplerStateOfTheDiffuseTexture()
        {
            var (quad, expander) = ShowTransformTexture();

            var sampler = expander.GetVisualDescendants().OfType<Views.SamplerState>().Single();

            Assert.Same(quad.Texture.DiffuseTexture.SamplerState, sampler.DataContext);
        }
    }
}
