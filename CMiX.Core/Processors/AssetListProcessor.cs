// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Processors
{
    // The ID of a texture source that loads a file, and the path of the file.
    public readonly record struct AssetEntry(Guid Id, string Path);

    // Gives the texture sources that load a file, in the order of the repository.
    // A hidden source is in the list, because its file must load too.
    // The list is built again only after a source or a path changed.
    public class AssetListProcessor
    {
        private readonly ControlRepository repository;
        private AssetEntry[] entries = Array.Empty<AssetEntry>();

        public AssetListProcessor(ControlRepository repository)
        {
            this.repository = repository;
        }

        public IReadOnlyList<AssetEntry> GetAssets()
        {
            var textures = repository.Textures;
            var count = 0;
            var same = true;

            foreach (var texture in textures)
            {
                if (texture is not IAssetTextureSource source)
                    continue;

                same = same && count < entries.Length && entries[count] == EntryOf(source);
                count++;
            }

            if (same && count == entries.Length)
                return entries;

            entries = textures.OfType<IAssetTextureSource>().Select(EntryOf).ToArray();
            return entries;
        }

        private static AssetEntry EntryOf(IAssetTextureSource source) =>
            new(((IControl)source).ID, source.AssetSelector.FilePath.Value);
    }
}
