using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class PrefabManagerResetGuardTests
    {
        [Fact]
        public void ResetItem_NullControl_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(TouchBlob));
            var item = manager.ManagerData.Items[0];

            var exception = Record.Exception(() => manager.ResetItem(null));

            Assert.Null(exception);
            Assert.Same(item, manager.ManagerData.Items[0]);
        }

        [Fact]
        public void ResetItem_ControlNotInCollection_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(TouchBlob));
            var item = manager.ManagerData.Items[0];
            var otherManager = provider.GetRequiredService<PrefabManager>();
            otherManager.AddItem(typeof(TouchBlob));
            var foreignControl = otherManager.ManagerData.Items[0];

            var exception = Record.Exception(() => manager.ResetItem(foreignControl));

            Assert.Null(exception);
            Assert.Same(item, manager.ManagerData.Items[0]);
        }
    }
}
