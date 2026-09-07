// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using Material.Icons;

namespace CMiX.Studio.Avalonia.Themes
{
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
            { "DeleteLayer", MaterialIconKind.Delete },
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
            { "Network", MaterialIconKind.LanPending },
            { "Resources", MaterialIconKind.Database },
            { "Add", MaterialIconKind.Plus },
            { "Sub", MaterialIconKind.Minus },
            { "Rename", MaterialIconKind.RenameBox },
            { "Close", MaterialIconKind.Close },
            { "Mask", MaterialIconKind.CircleBox },
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
            { "DragHandler", MaterialIconKind.DragHorizontal },
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
            { "Unlinked", MaterialIconKind.CircleMedium },
        };
    }
}
