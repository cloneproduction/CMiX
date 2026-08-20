using System.Collections.Specialized;
using CMiX.Core.Compositing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Toggling one slot's Visibility must never Clear() VisibleItems: Clear raises a Reset
    // notification, and every standard ItemsControl/ComboBox treats Reset as "I don't know what
    // changed" and defensively drops its current selection, even when the selection was never one
    // of the removed items. See OutputMappingManager.UpdateVisibleItem.
    public class OutputMappingVisibleItemsTests
    {
        [Fact]
        public void TogglingOneSlot_OnlyRaisesAddOrRemove_NeverReset()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var manager = project.OutputMappingManager;

            var actions = new List<NotifyCollectionChangedAction>();
            manager.VisibleItems.CollectionChanged += (s, e) => actions.Add(e.Action);

            manager.Items[1].Visibility.Value = false;
            manager.Items[1].Visibility.Value = true;

            Assert.All(actions, a => Assert.NotEqual(NotifyCollectionChangedAction.Reset, a));
            Assert.Contains(NotifyCollectionChangedAction.Remove, actions);
            Assert.Contains(NotifyCollectionChangedAction.Add, actions);
        }

        [Fact]
        public void DisablingOneSlot_LeavesOtherSlotsInVisibleItems()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var manager = project.OutputMappingManager;

            var slot0 = manager.Items[0];
            var slot1 = manager.Items[1];

            slot1.Visibility.Value = false;

            Assert.Contains(slot0, manager.VisibleItems);
            Assert.DoesNotContain(slot1, manager.VisibleItems);
            Assert.Equal(9, manager.VisibleItems.Count);
        }
    }
}
