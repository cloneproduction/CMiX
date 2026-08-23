// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Material.Icons;

namespace CMiX.Studio.Avalonia.Themes
{
    // Avalonia has no XAML instantiable bitmap resource, so the keyed icon bitmaps
    // from the WPF Icons.xaml are registered in code with the same resource keys.
    public static class Icons
    {
        // Semantic key -> MaterialIconKind, consumed by AppIcon. Keeping this as one
        // central mapping means swapping a glyph later is a one-line edit here rather
        // than hunting through every consuming .axaml file. Includes a few keys with
        // no live usage site today (kept for reference, cost nothing).
        public static readonly IReadOnlyDictionary<string, MaterialIconKind> Kinds = new Dictionary<string, MaterialIconKind>
        {
            { "Copy", MaterialIconKind.ContentCopy },
            { "Paste", MaterialIconKind.ContentPaste },
            { "Reset", MaterialIconKind.Restore },
            { "NewLayer", MaterialIconKind.LayersPlus },
            { "DeleteLayer", MaterialIconKind.LayersRemove },
            { "AddTab", MaterialIconKind.TabPlus },
            { "CloseTab", MaterialIconKind.TabRemove },
            { "Coloration", MaterialIconKind.PaletteSwatch },
            { "Geometry", MaterialIconKind.CubeOutline },
            { "PostFX", MaterialIconKind.AutoFix },
            { "Texture", MaterialIconKind.TextureBox },
            { "HSL", MaterialIconKind.Tune },
            { "Transform3D", MaterialIconKind.AxisArrow },
            { "BrightnessContrast", MaterialIconKind.ContrastBox },
            { "TexTransform", MaterialIconKind.Resize },
            { "Edit", MaterialIconKind.Pencil },
            { "Folder", MaterialIconKind.Folder },
            { "Folder_Opened", MaterialIconKind.FolderOpen },
            { "Network", MaterialIconKind.Lan },
            { "Resources", MaterialIconKind.FolderMultiple },
            { "Add", MaterialIconKind.Plus },
            { "Sub", MaterialIconKind.Minus },
            { "Rename", MaterialIconKind.RenameBox },
            { "Close", MaterialIconKind.Close },
            { "Mask", MaterialIconKind.Opacity },
            { "Cube", MaterialIconKind.Cube },
            { "CubeCol", MaterialIconKind.CubeScan },
            { "LayerScene", MaterialIconKind.Layers },
            { "Entity", MaterialIconKind.Shape },
            { "IsChecked", MaterialIconKind.Check },
            { "Composition", MaterialIconKind.ViewQuilt },
            { "Scene", MaterialIconKind.MovieOpen },
            { "Maximize", MaterialIconKind.WindowMaximize },
            { "Minimize", MaterialIconKind.WindowMinimize },
            { "Warning", MaterialIconKind.Alert },
            { "Settings", MaterialIconKind.Cog },
            { "Menu", MaterialIconKind.Menu },
            { "Play", MaterialIconKind.Play },
            { "Stop", MaterialIconKind.Stop },
            { "Visible", MaterialIconKind.Eye },
            { "NotVisible", MaterialIconKind.EyeOff },
            { "Scheduler", MaterialIconKind.CalendarClock },
            { "Playlist", MaterialIconKind.PlaylistMusic },
            { "DragHandler", MaterialIconKind.DragVertical },
            { "World", MaterialIconKind.Earth },
            { "MeshLight", MaterialIconKind.LightbulbSpot },
            { "ArrowUp", MaterialIconKind.ArrowUp },
            { "ArrowDown", MaterialIconKind.ArrowDown },
            { "ArrowRight", MaterialIconKind.ArrowRight },
            { "ArrowLeft", MaterialIconKind.ArrowLeft },
            // Distinct from the Arrow* keys above: chevrons for dropdown/expand
            // indicators (no directional-movement meaning), arrows for actual
            // move-item-up/down reordering buttons.
            { "ChevronUp", MaterialIconKind.ChevronUp },
            { "ChevronDown", MaterialIconKind.ChevronDown },
            { "ChevronRight", MaterialIconKind.ChevronRight },
            { "ChevronLeft", MaterialIconKind.ChevronLeft },
            { "Camera", MaterialIconKind.Camera },
            { "Beat", MaterialIconKind.Metronome },
            { "TextIcon", MaterialIconKind.FormatText },
        };

        private static readonly Dictionary<string, string> IconFiles = new()
        {
            { "Copy", "Copy.png" },
            { "Paste", "Paste.png" },
            { "Reset", "Reset.png" },
            { "NewLayer", "NewLayer.png" },
            { "DeleteLayer", "DeleteLayer.png" },
            { "AddTab", "AddTab.png" },
            { "CloseTab", "CloseTab.png" },
            { "Coloration", "Coloration.png" },
            { "Geometry", "Cube.png" },
            { "PostFX", "Effect.png" },
            { "Texture", "Texture.png" },
            { "HSL", "HSL.png" },
            { "Transform3D", "Transform3D.png" },
            { "BrightnessContrast", "BC.png" },
            { "TexTransform", "Size.png" },
            { "Edit", "Edit.png" },
            { "Folder", "Folder.png" },
            { "Folder_Opened", "Folder_Opened.png" },
            { "Network", "Network.png" },
            { "Resources", "Resource.png" },
            { "Add", "Add.png" },
            { "Sub", "Sub.png" },
            { "Rename", "Rename.png" },
            { "Close", "Close.png" },
            { "Mask", "Mask.png" },
            { "Cube", "Cube.png" },
            { "CubeCol", "Cube_Col.png" },
            { "LayerScene", "LayerScene.png" },
            { "Entity", "Entity.png" },
            { "IsChecked", "IsChecked.png" },
            { "Composition", "Composition.png" },
            { "Scene", "Scene.png" },
            { "Maximize", "Maximize.png" },
            { "Minimize", "Minimize.png" },
            { "Warning", "Warning.png" },
            { "Settings", "Settings.png" },
            { "Menu", "Menu.png" },
            { "Play", "Play.png" },
            { "Stop", "Stop.png" },
            { "Visible", "Visible.png" },
            { "NotVisible", "NotVisible.png" },
            { "Scheduler", "Scheduler.png" },
            { "Playlist", "Playlist.png" },
            { "ColorWheel", "ColorWheel.png" },
            { "CheckBoard", "Transparent_Bg.png" },
            { "DragHandler", "DragHandler.png" },
            { "World", "World.png" },
            { "MeshLight", "MeshLight.png" },
            { "ArrowUp", "Arrow_Up.png" },
            { "ArrowDown", "Arrow_Down.png" },
            { "ArrowRight", "Arrow_Right.png" },
            { "ArrowLeft", "Arrow_Left.png" },
            { "Camera", "Camera.png" },
            { "Beat", "Beat.png" },
            { "TextIcon", "Text.png" }
        };

        public static void Register(IResourceDictionary resources)
        {
            foreach (var (key, file) in IconFiles)
            {
                var uri = new Uri($"avares://CMiX.Studio.Avalonia/Themes/Assets/{file}");
                resources[key] = new Bitmap(AssetLoader.Open(uri));
            }
        }
    }
}
