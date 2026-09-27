using CMiX.Core.BaseControls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CompositeResetAllTests
    {
        [Fact]
        public void Integer2_ResetAllCommand_ResetsBothAxesToTheirSetDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var x = provider.GetRequiredService<GenericValue<int>>();
            var y = provider.GetRequiredService<GenericValue<int>>();
            x.SetDefault(1);
            y.SetDefault(2);
            var integer2 = new Integer2(x, y);

            x.Value = 9;
            y.Value = 9;
            integer2.ResetAllCommand.Execute(null);

            Assert.Equal(1, x.Value);
            Assert.Equal(2, y.Value);
        }

        [Fact]
        public void Integer3_ResetAllCommand_ResetsAllThreeAxesToTheirSetDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var x = provider.GetRequiredService<GenericValue<int>>();
            var y = provider.GetRequiredService<GenericValue<int>>();
            var z = provider.GetRequiredService<GenericValue<int>>();
            x.SetDefault(1);
            y.SetDefault(2);
            z.SetDefault(3);
            var integer3 = new Integer3(x, y, z);

            x.Value = 9;
            y.Value = 9;
            z.Value = 9;
            integer3.ResetAllCommand.Execute(null);

            Assert.Equal(1, x.Value);
            Assert.Equal(2, y.Value);
            Assert.Equal(3, z.Value);
        }

        [Fact]
        public void Vector2_ResetAllCommand_ResetsBothAxesToTheirSetDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var x = provider.GetRequiredService<GenericValue<float>>();
            var y = provider.GetRequiredService<GenericValue<float>>();
            x.SetDefault(1f);
            y.SetDefault(2f);
            var vector2 = new Vector2(x, y);

            x.Value = 9f;
            y.Value = 9f;
            vector2.ResetAllCommand.Execute(null);

            Assert.Equal(1f, x.Value);
            Assert.Equal(2f, y.Value);
        }

        [Fact]
        public void Vector3_ResetAllCommand_ResetsAllThreeAxesToTheirSetDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var x = provider.GetRequiredService<GenericValue<float>>();
            var y = provider.GetRequiredService<GenericValue<float>>();
            var z = provider.GetRequiredService<GenericValue<float>>();
            x.SetDefault(1f);
            y.SetDefault(2f);
            z.SetDefault(3f);
            var vector3 = new Vector3(x, y, z);

            x.Value = 9f;
            y.Value = 9f;
            z.Value = 9f;
            vector3.ResetAllCommand.Execute(null);

            Assert.Equal(1f, x.Value);
            Assert.Equal(2f, y.Value);
            Assert.Equal(3f, z.Value);
        }
    }
}
