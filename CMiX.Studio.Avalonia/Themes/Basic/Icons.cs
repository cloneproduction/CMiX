// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CMiX.Studio.Avalonia.Themes
{
    // Avalonia has no XAML instantiable bitmap resource, so the keyed icon bitmaps
    // from the WPF Icons.xaml are registered in code with the same resource keys.
    public static class Icons
    {
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
