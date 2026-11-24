using Avalonia.Headless.XUnit;
using CMiX.Core.Assets;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Behaviors;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.UI.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Image = CMiX.Core.Texturing.Sources.Image;

namespace CMiX.Studio.Avalonia.Tests
{
    // Files dropped on a texture list become textures that point to the files.
    public class TextureDropBehaviorTests : IDisposable
    {
        private readonly TempFolder temp = new("drop");

        public void Dispose() => temp.Dispose();

        private sealed class Dropper : TextureDropBehavior
        {
            public void Drop(IReadOnlyList<string> paths) => OnPathsDropped(paths);
        }

        private static (PrefabManager Textures, UndoManager Undo, Dropper Dropper) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var textures = provider.GetRequiredService<PrefabManager>();
            var undo = provider.GetRequiredService<UndoManager>();
            return (textures, undo, new Dropper { PrefabManager = textures, PathDropService = provider.GetRequiredService<PathDropService>() });
        }

        [AvaloniaFact]
        public void ADroppedImage_BecomesAnImageTextureThatPointsToTheFile()
        {
            var (textures, _, dropper) = Show();
            var path = temp.File("one.png");

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

            dropper.Drop(new[] { temp.File("one.png") });
            undo.Undo();

            Assert.Empty(textures.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Drop_ThreeFiles_IsOneUndoStep()
        {
            var (textures, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { temp.File("one.png"), temp.File("two.png"), temp.File("clip.mp4") });
            Assert.Equal(3, textures.ManagerData.Items.Count);
            undo.Undo();

            Assert.Empty(textures.ManagerData.Items);
        }

        [AvaloniaFact]
        public void TheBehavior_AcceptsFilesAndFoldersByDefault()
        {
            Assert.Equal(FileDropMode.FilesAndFolders, new TextureDropBehavior().Mode);
        }

        [AvaloniaFact]
        public void ADroppedFolderWithImages_BecomesASequencePlayerThatPointsToTheFolder()
        {
            var (textures, _, dropper) = Show();
            var folder = temp.Folder("frames", "frame_0001.png", "frame_0002.png");

            dropper.Drop(new[] { folder });

            var player = Assert.IsType<SequencePlayer>(Assert.Single(textures.ManagerData.Items));
            Assert.Equal(folder, player.AssetSelector.Asset.FilePath);
            Assert.IsType<ImageSequence>(player.AssetSelector.Asset);
        }

        [AvaloniaFact]
        public void ADroppedFolderWithoutImages_AddsNothing()
        {
            var (textures, _, dropper) = Show();

            dropper.Drop(new[] { temp.Folder("docs", "notes.txt") });

            Assert.Empty(textures.ManagerData.Items);
        }

        [AvaloniaFact]
        public void ADroppedFolderWithImages_IsOneUndoStep()
        {
            var (textures, undo, dropper) = Show();
            undo.Clear();

            dropper.Drop(new[] { temp.Folder("frames", "frame_0001.png") });
            undo.Undo();

            Assert.Empty(textures.ManagerData.Items);
        }

        [AvaloniaFact]
        public void ADroppedFileOfAnUnknownType_AddsNothing()
        {
            var (textures, _, dropper) = Show();

            dropper.Drop(new[] { temp.File("notes.txt") });

            Assert.Empty(textures.ManagerData.Items);
        }
    }
}
