using System.Collections.ObjectModel;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Controls is used from VL patches.
    public class ControlRepositoryVlMembersTests
    {
        private static (ControlFactory Factory, ControlRepository Repository, CollectionManager CollectionManager, IServiceProvider Provider) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider.GetRequiredService<ControlFactory>(), provider.GetRequiredService<ControlRepository>(),
                    provider.GetRequiredService<CollectionManager>(), provider);
        }

        [Fact]
        public void Controls_IsAnObservableCollectionOfIControl()
        {
            var (_, repository, _, _) = Create();

            Assert.IsType<ObservableCollection<IControl>>(repository.Controls);
        }
    }
}
