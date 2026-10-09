// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Assets
{
    // The kind of asset a file extension resolves to. Kept separate from any concrete asset or
    // texture source type, because AssetManager and AssetSelector construct IAsset instances while
    // TextureDropBehavior constructs texture source instances from the same extension list.
    public enum AssetKind
    {
        Image,
        Geometry,
        Video
    }

    // Single source of truth for the extension to asset kind mapping, consumed by
    // AssetManager.CreateAssetFromPath, AssetSelector.CreateAssetFromPath and
    // TextureDropBehavior.OnDrop, which each used to keep their own extension list.
    public static class AssetTypes
    {
        private static readonly Dictionary<string, AssetKind> Extensions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "DDS", AssetKind.Image },
            { "PNG", AssetKind.Image },
            { "JPG", AssetKind.Image },
            { "JPEG", AssetKind.Image },
            { "BMP", AssetKind.Image },
            { "OBJ", AssetKind.Geometry },
            { "FBX", AssetKind.Geometry },
            { "MOV", AssetKind.Video },
            { "MP4", AssetKind.Video },
            { "AVI", AssetKind.Video }
        };

        public static bool TryResolve(string path, out AssetKind kind)
        {
            var extension = Path.GetExtension(path).TrimStart('.');
            return Extensions.TryGetValue(extension, out kind);
        }

        public static IReadOnlyList<string> AllExtensions => Extensions.Keys.ToArray();
    }
}
