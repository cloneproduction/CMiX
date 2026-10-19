using Avalonia.Headless.XUnit;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Image = CMiX.Core.Texturing.Sources.Image;

namespace CMiX.Studio.Avalonia.Tests
{
    // Files dropped on a texture list become textures that point to the files.
    public class TextureDropBehaviorTests : IDisposable
    {
        private readonly string _folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cmix-drop-" + Guid.NewGuid().ToString("N"));

        public TextureDropBehaviorTests()
        {
            Directory.CreateDirectory(_folder);
        }

        public void Dispose() => Directory.Delete(_folder, true);

        private sealed class Dropper : TextureDropBehavior
        {
            public void Drop(IReadOnlyList<string> paths) => OnPathsDropped(paths);
        }

        private string NewFile(string name)
        {
            var path = System.IO.Path.Combine(_folder, name);
            File.WriteAllText(path, "x");
            return path;
        }

        private static (PrefabManager Textures, UndoManager Undo, Dropper Dropper) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var textures = provider.GetRequiredService<PrefabManager>();
            var undo = provider.GetRequiredService<UndoManager>();
            return (textures, undo, new Dropper { PrefabManager = textures });
        }

        [AvaloniaFact]
        public void ADroppedImage_BecomesAnImageTextureThatPointsToTheFile()
        {
            var (textures, _, dropper) = Show();
            var path = NewFile("one.png");

            dropper.Drop(new[] { path });

            var image = Assert.IsType<Image>(Assert.Single(textures.ManagerData.Items));
            Assert.Equal(path, image.AssetSelector.Asset.FilePath);
            Assert.Same(image, textures.SelectedItem);
        }

        [AvaloniaFact]
        public void ADroppedImage_IsOneUndoStep()
        {
            var (textures, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFile("one.png") });
            undo.Undo();

            Assert.Empty(textures.ManagerData.Items);
        }

        [AvaloniaFact]
        public void TwoDroppedImages_AreTwoUndoSteps()
        {
            var (textures, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { NewFile("one.png"), NewFile("two.png") });
            undo.Undo();

            Assert.Single(textures.ManagerData.Items);
        }

        [AvaloniaFact]
        public void ADroppedFileOfAnUnknownType_AddsNothing()
        {
            var (textures, _, dropper) = Show();

            dropper.Drop(new[] { NewFile("notes.txt") });

            Assert.Empty(textures.ManagerData.Items);
        }
    }
}
