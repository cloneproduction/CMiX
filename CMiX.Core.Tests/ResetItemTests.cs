// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // ResetItem swaps a fresh instance in. The undo command owns the replaced instance and tears it down when it is dropped.
    public class ResetItemTests
    {
        private static (Composition composition, Layer layer, Entity entity) CompositionWithALayerAndAnEntity(PrefabManager compositionManager)
        {
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            return (composition, layer, entity);
        }

        [Fact]
        public void ResetItem_OfALayer_SwapsAFreshInstanceInAtTheSameIndex()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (composition, layer, _) = CompositionWithALayerAndAnEntity(provider.GetRequiredService<PrefabManager>());
            composition.LayerManager.AddItem(typeof(Layer));
            var secondLayer = (Layer)composition.LayerManager.SelectedItem;

            composition.LayerManager.ResetItem(layer);

            var items = composition.LayerManager.ManagerData.Items;
            Assert.Equal(2, items.Count);
            Assert.NotSame(layer, items[0]);
            Assert.IsType<Layer>(items[0]);
            Assert.Same(secondLayer, items[1]);
            Assert.Same(items[0], composition.LayerManager.SelectedItem);
            Assert.NotNull(repository.GetControl(items[0].ID));
            Assert.Null(repository.GetControl(layer.ID));
        }

        [Fact]
        public void Undo_OfAResetItem_RestoresTheReplacedInstanceWithItsContents()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (composition, layer, entity) = CompositionWithALayerAndAnEntity(provider.GetRequiredService<PrefabManager>());

            composition.LayerManager.ResetItem(layer);
            var replacement = composition.LayerManager.ManagerData.Items[0];

            undoManager.Undo();

            Assert.Same(layer, composition.LayerManager.ManagerData.Items[0]);
            Assert.Contains(entity, layer.ModelEntityManager.ManagerData.Items);
            Assert.Same(entity, repository.GetControl(entity.ID));
            Assert.Null(repository.GetControl(replacement.ID));
        }

        [Fact]
        public void Clear_OfTheUndoEntryOfAResetItem_TearsTheReplacedInstanceDown()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (composition, layer, entity) = CompositionWithALayerAndAnEntity(provider.GetRequiredService<PrefabManager>());

            composition.LayerManager.ResetItem(layer);
            var replacement = composition.LayerManager.ManagerData.Items[0];
            undoManager.Clear();

            Assert.Empty(layer.ModelEntityManager.ManagerData.Items);
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Null(repository.GetControl(layer.ID));
            Assert.Same(replacement, composition.LayerManager.ManagerData.Items[0]);
            Assert.NotNull(repository.GetControl(replacement.ID));
        }

        [Fact]
        public void ResetItem_OfNull_DoesNothing()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(TouchBlob));
            var item = manager.ManagerData.Items[0];

            var exception = Record.Exception(() => manager.ResetItem(null));

            Assert.Null(exception);
            Assert.Same(item, manager.ManagerData.Items[0]);
        }

        [Fact]
        public void ResetItem_OfAControlOfAnotherList_DoesNothing()
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
