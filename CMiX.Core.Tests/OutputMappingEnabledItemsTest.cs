using System.Collections.Specialized;
using CMiX.Core.Compositing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Toggling one slot's IsEnabled must never Clear() EnabledItems: Clear raises a Reset
    // notification, and every standard ItemsControl/ComboBox treats Reset as "I don't know what
    // changed" and defensively drops its current selection, even when the selection was never one
    // of the removed items. See OutputMappingManager.UpdateEnabledItem.
    public class OutputMappingEnabledItemsTest
    {
        [Fact]
        public void TogglingOneSlot_OnlyRaisesAddOrRemove_NeverReset()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var manager = project.OutputMappingManager;

            var actions = new List<NotifyCollectionChangedAction>();
            manager.EnabledItems.CollectionChanged += (s, e) => actions.Add(e.Action);

            manager.Items[1].IsEnabled.Value = false;
            manager.Items[1].IsEnabled.Value = true;

            Assert.All(actions, a => Assert.NotEqual(NotifyCollectionChangedAction.Reset, a));
            Assert.Contains(NotifyCollectionChangedAction.Remove, actions);
            Assert.Contains(NotifyCollectionChangedAction.Add, actions);
        }

        [Fact]
        public void DisablingOneSlot_LeavesOtherSlotsInEnabledItems()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var manager = project.OutputMappingManager;

            var slot0 = manager.Items[0];
            var slot1 = manager.Items[1];

            slot1.IsEnabled.Value = false;

            Assert.Contains(slot0, manager.EnabledItems);
            Assert.DoesNotContain(slot1, manager.EnabledItems);
            Assert.Equal(9, manager.EnabledItems.Count);
        }
    }
}
