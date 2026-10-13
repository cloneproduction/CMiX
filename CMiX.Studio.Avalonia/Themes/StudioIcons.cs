// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Material.Icons;

namespace CMiX.Studio.Avalonia.Themes
{
    // The icon keys that name a part of the Studio. App.Initialize calls Register before the XAML loads.
    public static class StudioIcons
    {
        public static void Register()
        {
            Icons.Register("NewLayer", MaterialIconKind.LayersPlus);
            Icons.Register("DeleteLayer", MaterialIconKind.Delete);
            Icons.Register("Coloration", MaterialIconKind.PaletteSwatch);
            Icons.Register("Geometry", MaterialIconKind.CubeOutline);
            Icons.Register("PostFX", MaterialIconKind.AutoFix);
            Icons.Register("Texture", MaterialIconKind.TextureBox);
            Icons.Register("HSL", MaterialIconKind.Tune);
            Icons.Register("Transform3D", MaterialIconKind.AxisArrow);
            Icons.Register("BrightnessContrast", MaterialIconKind.ContrastBox);
            Icons.Register("TexTransform", MaterialIconKind.Resize);
            Icons.Register("Network", MaterialIconKind.LanPending);
            Icons.Register("Resources", MaterialIconKind.Database);
            Icons.Register("Mask", MaterialIconKind.CircleBox);
            Icons.Register("Cube", MaterialIconKind.Cube);
            Icons.Register("CubeCol", MaterialIconKind.CubeScan);
            Icons.Register("LayerScene", MaterialIconKind.Layers);
            Icons.Register("Entity", MaterialIconKind.Shape);
            Icons.Register("FullscreenQuad", MaterialIconKind.Fullscreen);
            Icons.Register("Composition", MaterialIconKind.ViewQuilt);
            Icons.Register("Scene", MaterialIconKind.MovieOpen);
            Icons.Register("Scheduler", MaterialIconKind.CalendarClock);
            Icons.Register("Playlist", MaterialIconKind.PlaylistMusic);
            Icons.Register("World", MaterialIconKind.Earth);
            Icons.Register("MeshLight", MaterialIconKind.LightbulbSpot);
            Icons.Register("Camera", MaterialIconKind.Camera);
            Icons.Register("Beat", MaterialIconKind.Metronome);
            Icons.Register("TextIcon", MaterialIconKind.FormatText);
        }
    }
}
