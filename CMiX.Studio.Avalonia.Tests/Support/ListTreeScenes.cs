// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The scenes and the finders that the pure tests of the new list tree share.
    internal static class ListTreeScenes
    {
        // Layer 1 of A gets two entities with modifiers, a camera and a filter. Layer 2 of A gets a filter. Layer 2 of B gets one entity.
        public static void Fill(Scene scene)
        {
            var layer1 = scene.LayerOfA(0);
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.CameraManager.AddItem(typeof(Camera));
            layer1.TextureModifierManager.AddItem(typeof(Blur));
            var first = (Entity)layer1.ModelEntityManager.ManagerData.Items[0];
            first.ModifierManager.AddItem(typeof(TranslateModifier));
            first.ModifierManager.AddItem(typeof(ScaleModifier));
            var second = (Entity)layer1.ModelEntityManager.ManagerData.Items[1];
            second.ModifierManager.AddItem(typeof(TranslateModifier));
            scene.LayerOfA(1).TextureModifierManager.AddItem(typeof(Blur));
            LayerOfB(scene, 1).ModelEntityManager.AddItem(typeof(Entity));
        }

        // The scene of Fill, and the second entity of layer 1 of A shows under layer 2 of A too.
        public static void ShareAnEntity(Scene scene)
        {
            Fill(scene);
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem((IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[1]);
        }

        // Opens every row, and closes every empty list.
        public static Scene Opened(Scene scene)
        {
            foreach (var node in scene.All())
                node.IsExpanded = node.Group == null || node.Children.Count > 0;

            return scene;
        }

        public static Layer LayerOfB(Scene scene, int index) => (Layer)scene.B.LayerManager.ManagerData.Items[index];

        public static TreeNode Entity(Scene scene, int index) => scene.Group<EntitiesGroup>(0).Children[index];

        public static TreeNode Modifier(Scene scene, int entity, int index) => Scene.Group<ModifiersGroup>(Entity(scene, entity)).Children[index];

        public static TreeNode EntityOfB(Scene scene) => Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(1)).Children[0];

        // The rows and every row under them, in tree order.
        public static IEnumerable<ListTreeRow> AllRows(IEnumerable<ListTreeRow> rows) =>
            rows.SelectMany(row => AllRows(row.Children).Prepend(row));
    }
}
