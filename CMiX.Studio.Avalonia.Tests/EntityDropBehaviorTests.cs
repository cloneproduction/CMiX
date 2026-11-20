using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CMiX.Core;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Behaviors;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FullscreenQuad = CMiX.Core.Compositing.FullscreenQuad;
using Image = CMiX.Core.Texturing.Sources.Image;
using Layer = CMiX.Core.Compositing.Layer;
using TextEntity = CMiX.Core.Compositing.TextEntity;
using SequencePlayer = CMiX.Core.Texturing.Sources.SequencePlayer;
using VideoPlayer = CMiX.Core.Texturing.Sources.VideoPlayer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Files and folders dropped on an entity list become entities that point to them.
    public class EntityDropBehaviorTests : IDisposable
    {
        private readonly string _folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cmix-entity-drop-" + Guid.NewGuid().ToString("N"));

        public EntityDropBehaviorTests()
        {
            Directory.CreateDirectory(_folder);
        }

        public void Dispose() => Directory.Delete(_folder, true);

        private sealed class Dropper : EntityDropBehavior
        {
            public void Drop(IReadOnlyList<string> paths) => OnPathsDropped(paths);
        }

        private string NewFile(string name)
        {
            var path = System.IO.Path.Combine(_folder, name);
            File.WriteAllText(path, "x");
            return path;
        }

        private string NewFolder(string name, params string[] files)
        {
            var folder = System.IO.Path.Combine(_folder, name);
            Directory.CreateDirectory(folder);
            foreach (var file in files)
                File.WriteAllText(System.IO.Path.Combine(folder, file), "x");
            return folder;
        }

        private static (PrefabManager Entities, UndoManager Undo, Dropper Dropper) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var entities = provider.GetRequiredService<PrefabManager>();
            var undo = provider.GetRequiredService<UndoManager>();
            return (entities, undo, new Dropper { PrefabManager = entities, PathDropService = provider.GetRequiredService<PathDropService>() });
        }

        private static IAssetTextureSource TextureOf(ITexturable textured) =>
            Assert.IsAssignableFrom<IAssetTextureSource>(
                Assert.Single(textured.Texture.DiffuseTexture.TextureManager.ManagerData.Items));

        [AvaloniaFact]
        public void Create_TheBehavior_AcceptsFilesAndFolders()
        {
            Assert.Equal(FileDropMode.FilesAndFolders, new EntityDropBehavior().Mode);
        }

        [AvaloniaFact]
        public void Show_TheEntityListOfALayer_HasTheDropBehaviorOnItsManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var tab = new TabLayerEntity { DataContext = layer };
            var window = new Window { Content = tab, Width = 500, Height = 900 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var slot = tab.GetVisualDescendants().OfType<ListPrefabSlot>().First();
            var behavior = Assert.Single(Interaction.GetBehaviors(slot).OfType<EntityDropBehavior>());

            Assert.Same(layer.ModelEntityManager, behavior.PrefabManager);
        }

        [AvaloniaFact]
        public void Drop_AFolderWithImages_AddsAQuadWithASequencePlayer()
        {
            var (entities, _, dropper) = Show();
            var folder = NewFolder("frames", "frame_0001.png");

            dropper.Drop(new[] { folder });

            var quad = Assert.IsType<FullscreenQuad>(Assert.Single(entities.ManagerData.Items));
            var source = TextureOf(quad);
            Assert.IsType<SequencePlayer>(source);
            Assert.Equal(folder, source.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void Drop_AFolderWithoutImages_AddsNothing()
        {
            var (entities, _, dropper) = Show();

            dropper.Drop(new[] { NewFolder("docs", "notes.txt") });

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_AnImage_AddsAnEntityWithTheImage()
        {
            var (entities, _, dropper) = Show();
            var path = NewFile("one.png");

            dropper.Drop(new[] { path });

            var entity = Assert.IsType<Entity>(Assert.Single(entities.ManagerData.Items));
            var source = TextureOf(entity);
            Assert.IsType<Image>(source);
            Assert.Equal(path, source.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void Drop_AVideo_AddsAnEntityWithAVideoPlayer()
        {
            var (entities, _, dropper) = Show();
            var path = NewFile("clip.mp4");

            dropper.Drop(new[] { path });

            var entity = Assert.IsType<Entity>(Assert.Single(entities.ManagerData.Items));
            var source = TextureOf(entity);
            Assert.IsType<VideoPlayer>(source);
            Assert.Equal(path, source.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void Drop_AGeometryFile_AddsAnEntityWithThatMesh()
        {
            var (entities, _, dropper) = Show();
            var path = NewFile("model.obj");

            dropper.Drop(new[] { path });

            var entity = Assert.IsType<Entity>(Assert.Single(entities.ManagerData.Items));
            Assert.Equal(MeshType.FromFile, entity.Mesh.MeshTypeSelector.Value);
            Assert.Equal(path, entity.Mesh.Geometry.Asset.FilePath);
            Assert.IsType<Geometry>(entity.Mesh.Geometry.Asset);
        }

        [AvaloniaFact]
        public void Drop_AFileOfAnUnknownType_AddsNothing()
        {
            var (entities, _, dropper) = Show();

            dropper.Drop(new[] { NewFile("notes.xyz") });

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_ATextFile_AddsATextEntityWithTheTextOfTheFile()
        {
            var (entities, _, dropper) = Show();
            var path = System.IO.Path.Combine(_folder, "notes.txt");
            File.WriteAllText(path, "Hello\nWorld");

            dropper.Drop(new[] { path });

            var entity = Assert.IsType<TextEntity>(Assert.Single(entities.ManagerData.Items));
            Assert.Equal("Hello\nWorld", entity.Text.Value);
        }

        [AvaloniaFact]
        public void Drop_ATextFile_IsOneUndoStep()
        {
            var (entities, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFile("notes.txt") });
            undo.Undo();

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_ATextFileThatIsTooLarge_AddsNothing()
        {
            var (entities, _, dropper) = Show();
            var path = System.IO.Path.Combine(_folder, "big.txt");
            File.WriteAllText(path, new string('a', (int)Studio.Avalonia.Services.PathDropService.MaxTextBytes + 1));

            dropper.Drop(new[] { path });

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_AnImage_IsOneUndoStep()
        {
            var (entities, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFile("one.png") });
            undo.Undo();

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_AFolderWithImages_IsOneUndoStep()
        {
            var (entities, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFolder("frames", "frame_0001.png") });
            undo.Undo();

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_AGeometryFile_IsOneUndoStep()
        {
            var (entities, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFile("model.obj") });
            undo.Undo();

            Assert.Empty(entities.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_ThreeFiles_IsOneUndoStep()
        {
            var (entities, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFile("one.png"), NewFile("model.obj"), NewFile("notes.txt") });
            Assert.Equal(3, entities.ManagerData.Items.Count);
            undo.Undo();

            Assert.Empty(entities.ManagerData.Items);
        }
    }
}
