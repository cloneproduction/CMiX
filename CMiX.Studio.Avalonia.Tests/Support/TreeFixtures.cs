using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Notices;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // Builds a project and its tree for the tree tests.
    internal static class TreeFixtures
    {
        // A project, its tree and its first two compositions. A and B are null when the project does not hold them.
        internal sealed record Scene
        {
            public IServiceProvider Provider { get; init; } = null!;
            public Project Project { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;
            public ProjectTree Tree { get; init; } = null!;
            public Composition A { get; init; } = null!;
            public Composition B { get; init; } = null!;

            public void ClearUndo() => Undo.Clear();

            public Composition Composition => A;
            public Layer Layer => LayerOfA(0);
            public Layer LayerOfA(int index) => (Layer)A.LayerManager.ManagerData.Items[index];

            public TreeNode NodeA => Tree.CompositionNodes()[0];
            public TreeNode NodeB => Tree.CompositionNodes()[1];
            public TreeNode CompositionNode => NodeA;
            public TreeNode LayerNode => LayerNodeOfA(0);
            public TreeNode LayerNodeOfA(int index) => LayersOf(NodeA).Children[index];
            public TreeNode LayerNodeOfB(int index) => LayersOf(NodeB).Children[index];

            // The row of the layers of a composition: the Layers group, the first row under the composition.
            public static TreeNode LayersOf(TreeNode composition) => Group<LayersGroup>(composition);

            // The group of this kind under a node.
            public static TreeNode Group<T>(TreeNode node) where T : TreeGroup => node.Children.Single(child => child.Group is T);
            public TreeNode Group<T>(int layerIndex) where T : TreeGroup => Group<T>(LayerNodeOfA(layerIndex));
            public TreeNode Group(int layerIndex, Type kind) => LayerNodeOfA(layerIndex).Children.Single(group => kind.IsInstanceOfType(group.Group));

            // The headers of the groups under a node.
            public static string[] Headers(TreeNode node) => node.Children.Where(child => child.Item == null).Select(child => child.Header).ToArray();

            // Every node of the tree, in tree order.
            public IEnumerable<TreeNode> All() => ListTreeTestScreen.All(Tree.Roots);
        }

        // Adds one composition per value of layers, with that number of layers. The default is A with two layers and B with one.
        // fill adds more items before the tree is made.
        public static Scene CreateScene(int[]? layers = null, Action<Scene>? fill = null)
        {
            layers ??= [2, 1];
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            foreach (var _ in layers)
                project.CompositionManager.AddItem(typeof(Composition));
            for (var i = 0; i < layers.Length; i++)
            {
                var composition = (Composition)project.CompositionManager.ManagerData.Items[i];
                for (var j = 0; j < layers[i]; j++)
                    composition.LayerManager.AddItem(typeof(Layer));
            }

            var undo = provider.GetRequiredService<UndoManager>();
            var scene = new Scene
            {
                Provider = provider,
                Project = project,
                Undo = undo,
                A = layers.Length > 0 ? (Composition)project.CompositionManager.ManagerData.Items[0] : null!,
                B = layers.Length > 1 ? (Composition)project.CompositionManager.ManagerData.Items[1] : null!
            };
            fill?.Invoke(scene);
            return scene with { Tree = CreateTree(provider) };
        }

        // A new tree of the project of the provider, with the services of the provider.
        public static ProjectTree CreateTree(IServiceProvider provider) =>
            new(provider.GetRequiredService<Project>(),
                provider.GetRequiredService<UndoManager>(),
                provider.GetRequiredService<IMoveBlockedNotice>(),
                provider.GetRequiredService<ShortcutMap>(),
                provider.GetRequiredService<PathDropService>(),
                provider.GetRequiredService<INotificationService>(),
                provider.GetRequiredService<InterfaceOptions>());

        // Adds two entities, one filter and three modifiers to the layer.
        public static void AddKinds(Layer layer)
        {
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.TextureModifierManager.AddItem(typeof(Blur));
            for (var i = 0; i < 3; i++)
                layer.ModifierManager.AddItem(typeof(TranslateModifier));
        }
    }
}
