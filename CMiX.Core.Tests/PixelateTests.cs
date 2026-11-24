using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class PixelateTests
    {
        [Fact]
        public void XAndY_DefaultToPointFive()
        {
            var provider = TestServiceProviderFactory.Create();
            var pixelate = (Pixelate)provider.GetRequiredService<ControlFactory>().Create(typeof(Pixelate));

            Assert.Equal(0.5f, pixelate.X.Value);
            Assert.Equal(0.5f, pixelate.Y.Value);
        }
    }
}
