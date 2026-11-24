using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    file static class SceneOfB
    {
        public static Layer LayerOfB(this Scene scene) => (Layer)scene.B.LayerManager.ManagerData.Items[0];
    }

    // The rules for a drag in the project tree: the same type in the same list, a layer to another composition.
    public class TreeDropServiceTests
    {
        private readonly TreeDropService service = new();

        [Fact]
        public void Drop_ACompositionAfterAnother_ReordersTheCompositions()
        {
            var scene = CreateScene();

            var outcome = service.Drop(scene.NodeA, scene.NodeB, DropPosition.After);

            Assert.True(outcome.Moved);
            Assert.Equal(new IControl[] { scene.B, scene.A }, scene.Project.CompositionManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_ACompositionBeforeAnother_ReordersTheCompositions()
        {
            var scene = CreateScene();

            service.Drop(scene.NodeB, scene.NodeA, DropPosition.Before);

            Assert.Equal(new IControl[] { scene.B, scene.A }, scene.Project.CompositionManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_ALayerAfterTheNextLayer_ReordersTheLayers()
        {
            var scene = CreateScene();
            var first = scene.LayerOfA(0);
            var second = scene.LayerOfA(1);

            service.Drop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.After);

            Assert.Equal(new IControl[] { second, first }, scene.A.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_AnItemWhereItAlreadyIs_DoesNothing()
        {
            var scene = CreateScene();

            Assert.False(service.Drop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.Before).Moved);
            Assert.False(service.Drop(scene.LayerNodeOfA(1), scene.LayerNodeOfA(0), DropPosition.After).Moved);
        }

        [Fact]
        public void CanDrop_WhereTheItemAlreadyIs_IsFalse()
        {
            var scene = CreateScene();

            // The first layer before the second layer, or after itself, or before itself: the same place.
            Assert.False(service.CanDrop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.Before));
            Assert.False(service.CanDrop(scene.LayerNodeOfA(1), scene.LayerNodeOfA(0), DropPosition.After));
            Assert.False(service.CanDrop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(0), DropPosition.Before));
            Assert.False(service.CanDrop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(0), DropPosition.After));
        }

        [Fact]
        public void CanDrop_ToAnotherPlace_IsTrue()
        {
            var scene = CreateScene();

            Assert.True(service.CanDrop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.After));
            Assert.True(service.CanDrop(scene.LayerNodeOfA(1), scene.LayerNodeOfA(0), DropPosition.Before));
        }

        [Fact]
        public void CanDrop_ALayerInsideItsOwnComposition_IsTrueOnlyWhileItIsNotTheLastLayer()
        {
            var scene = CreateScene();

            Assert.True(service.CanDrop(scene.LayerNodeOfA(0), scene.NodeA, DropPosition.Inside));
            Assert.False(service.CanDrop(scene.LayerNodeOfA(1), scene.NodeA, DropPosition.Inside));
        }

        [Fact]
        public void CanDrop_ALayerToAnotherComposition_IsTrueEvenAtTheSameIndex()
        {
            var scene = CreateScene();

            Assert.True(service.CanDrop(scene.LayerNodeOfA(0), scene.NodeB, DropPosition.Inside));
            Assert.True(service.CanDrop(scene.LayerNodeOfA(0), scene.LayerNodeOfB(0), DropPosition.Before));
        }

        [Fact]
        public void Drop_AnEntityBeforeAnotherEntity_ReordersTheGroup()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var first = layer.ModelEntityManager.ManagerData.Items[0];
            var second = layer.ModelEntityManager.ManagerData.Items[1];
            var group = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0));

            service.Drop(group.Children[1], group.Children[0], DropPosition.Before);

            Assert.Equal(new IControl[] { second, first }, layer.ModelEntityManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_ALayerBeforeALayerOfAnotherComposition_MovesTheLayerThere()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);

            var outcome = service.Drop(scene.LayerNodeOfA(0), scene.LayerNodeOfB(0), DropPosition.Before);

            Assert.True(outcome.Moved);
            Assert.Single(scene.A.LayerManager.ManagerData.Items);
            Assert.Same(moved, scene.B.LayerManager.ManagerData.Items[0]);
        }

        [Fact]
        public void Drop_ALayerInsideAComposition_MovesTheLayerToTheEnd()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);

            service.Drop(scene.LayerNodeOfA(0), scene.NodeB, DropPosition.Inside);

            Assert.Equal(2, scene.B.LayerManager.ManagerData.Items.Count);
            Assert.Same(moved, scene.B.LayerManager.ManagerData.Items[1]);
        }

        [Fact]
        public void Drop_ALayerInsideItsOwnComposition_MovesTheLayerToTheEnd()
        {
            var scene = CreateScene();
            var first = scene.LayerOfA(0);

            service.Drop(scene.LayerNodeOfA(0), scene.NodeA, DropPosition.Inside);

            Assert.Same(first, scene.A.LayerManager.ManagerData.Items[1]);
        }

        [Fact]
        public void Drop_ALayerWithASharedEntity_StaysAndReturnsTheBlockingItems()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)moved.ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);

            var outcome = service.Drop(scene.LayerNodeOfA(0), scene.NodeB, DropPosition.Inside);

            Assert.False(outcome.Moved);
            Assert.Same(entity, Assert.Single(outcome.Blocking));
            Assert.Equal(2, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.Single(scene.B.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void CanDrop_ALayerWithASharedEntity_IsTrueSoTheUserCanReadTheReason()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem((IPrefab)moved.ModelEntityManager.ManagerData.Items[0]);

            Assert.True(service.CanDrop(scene.LayerNodeOfA(0), scene.NodeB, DropPosition.Inside));
        }

        [Fact]
        public void DescribeBlock_ALayerWithSharedItems_NamesTheLayerTheCompositionsAndTheItems()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)moved.ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);

            var text = service.DescribeBlock(scene.LayerNodeOfA(0), scene.NodeB, new[] { entity });

            Assert.Contains($"\"{scene.LayerNodeOfA(0).Header}\"", text);
            Assert.Contains($"\"{scene.NodeA.Header}\"", text);
            Assert.Contains($"\"{scene.NodeB.Header}\"", text);
            Assert.Contains(entity.PrefabService.Name.Value, text);
            Assert.DoesNotContain((char)0x2013, text);
            Assert.DoesNotContain((char)0x2014, text);
        }


        [Fact]
        public void CanDrop_ACompositionInsideAComposition_IsFalse()
        {
            var scene = CreateScene();

            Assert.False(service.CanDrop(scene.NodeA, scene.NodeB, DropPosition.Inside));
        }

        [Fact]
        public void CanDrop_ALayerInsideALayerOrAGroup_IsFalse()
        {
            var scene = CreateScene();

            Assert.False(service.CanDrop(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.Inside));
            Assert.False(service.CanDrop(scene.LayerNodeOfA(0), Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)), DropPosition.Inside));
        }

        [Fact]
        public void CanDrop_ACompositionBetweenLayers_IsFalse()
        {
            var scene = CreateScene();

            Assert.False(service.CanDrop(scene.NodeA, scene.LayerNodeOfB(0), DropPosition.Before));
        }

        [Fact]
        public void Drop_AnEntityBeforeAnEntityOfAnotherLayer_MovesItThere()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
            var moved = scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            var staying = scene.LayerOfA(1).ModelEntityManager.ManagerData.Items[0];
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var target = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)).Children[0];

            Assert.True(service.CanDrop(source, target, DropPosition.Before));
            var outcome = service.Drop(source, target, DropPosition.Before);

            Assert.True(outcome.Moved);
            Assert.Empty(scene.LayerOfA(0).ModelEntityManager.ManagerData.Items);
            Assert.Equal(new IControl[] { moved, staying }, scene.LayerOfA(1).ModelEntityManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_AnEntityInsideTheEntitiesOfAnotherLayer_MovesItToTheEnd()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
            var moved = scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            var staying = scene.LayerOfA(1).ModelEntityManager.ManagerData.Items[0];
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var target = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1));

            Assert.True(service.CanDrop(source, target, DropPosition.Inside));
            service.Drop(source, target, DropPosition.Inside);

            Assert.Equal(new IControl[] { staying, moved }, scene.LayerOfA(1).ModelEntityManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_AnEntityInsideAnEmptyEntitiesOfAnotherLayer_MovesIt()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var moved = scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];

            service.Drop(Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0], Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)), DropPosition.Inside);

            Assert.Same(moved, Assert.Single(scene.LayerOfA(1).ModelEntityManager.ManagerData.Items));
        }

        [Fact]
        public void CanDrop_AnEntityIntoACameraGroupOrItsOwnGroup_IsFalse()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];

            Assert.False(service.CanDrop(source, Scene.Group<CamerasGroup>(scene.LayerNodeOfA(1)), DropPosition.Inside));
            Assert.False(service.CanDrop(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)), DropPosition.Inside));
        }

        [Fact]
        public void CanDrop_ACameraIntoTheCamerasOfAnotherLayer_IsTrue()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).CameraManager.AddItem(typeof(Camera));
            var source = Scene.Group<CamerasGroup>(scene.LayerNodeOfA(0)).Children[0];

            Assert.True(service.CanDrop(source, Scene.Group<CamerasGroup>(scene.LayerNodeOfA(1)), DropPosition.Inside));
            Assert.False(service.CanDrop(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)), DropPosition.Inside));
        }

        [Fact]
        public void CanDrop_AnEntityThatTheOtherLayerHasAlready_IsFalse()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];

            Assert.False(service.CanDrop(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)), DropPosition.Inside));
        }

        [Fact]
        public void Drop_AnEntityToAnotherComposition_ChangesItsCompositionID()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IHasCompositionID)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];

            var outcome = service.Drop(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(0)), DropPosition.Inside);

            Assert.True(outcome.Moved);
            Assert.Equal(scene.B.ID, entity.CompositionID);
        }

        [Fact]
        public void Drop_AnEntityThatAnotherLayerUsesToAnotherComposition_StaysAndReturnsTheSharingLayers()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var target = Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(0));

            Assert.True(service.CanDrop(source, target, DropPosition.Inside));
            var outcome = service.Drop(source, target, DropPosition.Inside);

            Assert.False(outcome.Moved);
            Assert.Same(scene.LayerOfA(1), Assert.Single(outcome.Blocking));
            Assert.Contains(entity, scene.LayerOfA(0).ModelEntityManager.ManagerData.Items);
            Assert.Empty(scene.LayerOfB().ModelEntityManager.ManagerData.Items);
            var text = service.DescribeBlock(source, target, outcome.Blocking);
            Assert.Contains("also used by other layers", text);
            Assert.Contains($"\"{scene.LayerOfA(1).PrefabService.Name.Value}\"", text);
        }

        [Fact]
        public void Drop_AnEntityThatTwoOtherLayersUseToAnotherComposition_ReturnsBothLayers()
        {
            var scene = CreateScene();
            scene.A.LayerManager.AddItem(typeof(Layer));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);
            scene.LayerOfA(2).ModelEntityManager.AddExistingItem(entity);
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var target = Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(0));

            var outcome = service.Drop(source, target, DropPosition.Inside);

            Assert.False(outcome.Moved);
            Assert.Equal(new IPrefab[] { scene.LayerOfA(1), scene.LayerOfA(2) }, outcome.Blocking);
            var text = service.DescribeBlock(source, target, outcome.Blocking);
            Assert.Contains($"\"{scene.LayerOfA(1).PrefabService.Name.Value}\", \"{scene.LayerOfA(2).PrefabService.Name.Value}\"", text);
        }

        [Fact]
        public void Drop_AnEntityThatAnotherLayerUsesToAnotherLayerOfTheSameComposition_Moves()
        {
            var scene = CreateScene();
            scene.A.LayerManager.AddItem(typeof(Layer));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];

            var outcome = service.Drop(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(2)), DropPosition.Inside);

            Assert.True(outcome.Moved);
            Assert.Contains(entity, scene.LayerOfA(2).ModelEntityManager.ManagerData.Items);
            Assert.Contains(entity, scene.LayerOfA(1).ModelEntityManager.ManagerData.Items);
            Assert.DoesNotContain(entity, scene.LayerOfA(0).ModelEntityManager.ManagerData.Items);
        }

        [Fact]
        public void CanDrop_ACameraBeforeAnEntity_IsFalse()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);
            layer.CameraManager.AddItem(typeof(Camera));
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var camera = Scene.Group<CamerasGroup>(scene.LayerNodeOfA(0)).Children[0];
            var entity = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];

            Assert.False(service.CanDrop(camera, entity, DropPosition.Before));
        }

        [Fact]
        public void CanDrop_AGroup_CannotBeDragged()
        {
            var scene = CreateScene();
            var group = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0));

            Assert.False(service.CanDrop(group, scene.LayerNodeOfA(1), DropPosition.Before));
        }

        [Fact]
        public void Drop_ALayerThatTheTargetRefuses_ReportsNothingHappened()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            scene.B.LayerManager.CanAddExistingItem = _ => false;

            var outcome = service.Drop(scene.LayerNodeOfA(0), scene.LayerNodeOfB(0), DropPosition.After);

            Assert.Equal(DropOutcome.None, outcome);
            Assert.Contains(moved, scene.A.LayerManager.ManagerData.Items);
            Assert.DoesNotContain(moved, scene.B.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void Drop_ALayerSharedWithAnotherLayer_ReportsTheBlockingItems()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)moved.ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);

            var outcome = service.Drop(scene.LayerNodeOfA(0), scene.LayerNodeOfB(0), DropPosition.After);

            Assert.False(outcome.Moved);
            Assert.Same(entity, Assert.Single(outcome.Blocking));
        }

        [Fact]
        public void Accepts_AnOwnerThatCannotHoldTheType_IsFalse()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            entity.ModifierManager.AddItem(typeof(TranslateModifier));
            var modifier = Scene.Group<ModifiersGroup>(Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0]).Children[0];
            var entityModifiers = Scene.Group<ModifiersGroup>(Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)).Children[0]);
            var layerModifiers = Scene.Group<ModifiersGroup>(scene.LayerNodeOfB(0));

            Assert.False(ListRules.CanHold(typeof(Layer), typeof(TranslateModifier)));
            Assert.False(service.CanDrop(modifier, layerModifiers, DropPosition.Inside));
            Assert.True(service.CanDrop(modifier, entityModifiers, DropPosition.Inside));
        }

        [Fact]
        public void BlockReason_AnEntityThatAnotherLayerShares_NamesTheLayer()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var expected = $"Layer \"{scene.LayerOfA(1).PrefabService.Name.Value}\" also holds it";

            Assert.Equal(expected, service.BlockReason(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(0))));
            Assert.Equal(expected, service.BlockReason(source, scene.LayerNodeOfB(0)));
        }

        [Fact]
        public void BlockReason_AModifierForAnOwnerThatCannotHoldIt_GivesTheReason()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            entity.ModifierManager.AddItem(typeof(TranslateModifier));
            var modifier = Scene.Group<ModifiersGroup>(Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0]).Children[0];

            Assert.Equal("A Translate cannot go to a layer", service.BlockReason(modifier, Scene.Group<ModifiersGroup>(scene.LayerNodeOfB(0))));
            Assert.True(service.IsTargetByKind(modifier, Scene.Group<ModifiersGroup>(scene.LayerNodeOfB(0))));
            Assert.False(service.IsInsideTarget(modifier, Scene.Group<ModifiersGroup>(scene.LayerNodeOfB(0))));
        }

        [Fact]
        public void BlockReason_ATargetListThatRefusesTheItem_NamesTheList()
        {
            var scene = CreateScene();
            scene.B.LayerManager.CanAddExistingItem = _ => false;

            Assert.Equal($"\"{scene.NodeB.Header}\" cannot take \"{scene.LayerNodeOfA(0).Header}\"", service.BlockReason(scene.LayerNodeOfA(0), scene.NodeB));
        }

        [Fact]
        public void BlockReason_AnAllowedDrop_IsNull()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];

            Assert.Null(service.BlockReason(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(0))));
            Assert.Null(service.BlockReason(source, scene.LayerNodeOfB(0)));
            Assert.Null(service.BlockReason(source, Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0))));
            Assert.Null(service.BlockReason(scene.LayerNodeOfA(0), scene.NodeB));
        }

        [Fact]
        public void Drop_AnEntityOnALayerRow_GoesToTheEntitiesGroup()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            var source = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var layerRow = scene.LayerNodeOfB(0);

            Assert.Same(Scene.Group<EntitiesGroup>(layerRow), TreeDropService.InsideOf(source, layerRow));
            Assert.True(service.IsInsideTarget(source, layerRow));

            var outcome = service.Drop(source, layerRow, DropPosition.Inside);

            Assert.True(outcome.Moved);
            Assert.Same(entity, Assert.Single(scene.LayerOfB().ModelEntityManager.ManagerData.Items));
            Assert.Empty(scene.LayerOfA(0).ModelEntityManager.ManagerData.Items);
        }

        [Fact]
        public void CanDrop_BeforeOrAfterAGroup_IsFalse()
        {
            var scene = CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            var group = Scene.Group<CamerasGroup>(scene.LayerNodeOfA(0));

            Assert.False(service.CanDrop(entity, group, DropPosition.Before));
            Assert.False(service.CanDrop(entity, group, DropPosition.After));
        }

        [Fact]
        public void CanDrop_ANodeOntoItselfOrItsOwnChild_IsFalse()
        {
            var scene = CreateScene();

            Assert.False(service.CanDrop(scene.NodeA, scene.NodeA, DropPosition.Before));
            Assert.False(service.CanDrop(scene.NodeA, scene.LayerNodeOfA(0), DropPosition.Before));
        }

        [Fact]
        public void PlaceOf_ALayerAfterTheNextLayer_GivesTheListAndTheIndexBeforeTheMove()
        {
            var scene = CreateScene();

            var place = service.PlaceOf(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.After);

            Assert.Equal(new ListPlace(scene.A.LayerManager, 2), place);
            Assert.Null(service.PlaceOf(scene.LayerNodeOfA(0), scene.LayerNodeOfA(1), DropPosition.Before));
        }
    }
}
