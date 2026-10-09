// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later


namespace CMiX_ENGINEUtils
{
    /// <summary>
    /// Maps a path from another machine onto one of several local root folders.
    /// </summary>
    [ProcessNode]
    public class ResolvePath
    {
        // Cached inputs and outputs so the file system is only touched when something changes.
        // Nullable (?) because they are unset until the first Update.
        private string? _lastIncoming;
        private Spread<string>? _lastRoots;
        private bool _lastUseProbing;

        private string _localPath = "";
        private bool _found;
        private bool _isDirectory;
        private int _rootIndex = -1;

        /// <summary>
        /// Inputs:  Incoming Path, Local Roots (spread), Use Suffix Probing, Force
        /// Outputs: Local Path (return), Found, Is Directory, Root Index
        /// </summary>
        public string Update(
            string? incomingPath,
            Spread<string>? localRoots,
            bool useSuffixProbing,
            bool force,
            out bool found,
            out bool isDirectory,
            out int rootIndex)
        {
            // Spread<string> is immutable: a different reference means different content.
            bool inputsChanged =
                !string.Equals(incomingPath, _lastIncoming, StringComparison.Ordinal) ||
                !ReferenceEquals(localRoots, _lastRoots) ||
                useSuffixProbing != _lastUseProbing;

            if (inputsChanged || force)
            {
                _lastIncoming = incomingPath;
                _lastRoots = localRoots;
                _lastUseProbing = useSuffixProbing;

                var result = Resolve(incomingPath, localRoots, useSuffixProbing, out _rootIndex);

                _found = result != null;
                _localPath = result ?? "";
                _isDirectory = result != null && Directory.Exists(result);
            }

            found = _found;
            isDirectory = _isDirectory;
            rootIndex = _rootIndex;
            return _localPath;
        }

        // ---------- resolve logic ----------

        private static string? Resolve(string? incomingPath, Spread<string>? localRoots, bool useSuffixProbing, out int rootIndex)
        {
            rootIndex = -1;

            if (string.IsNullOrWhiteSpace(incomingPath) || localRoots == null || localRoots.Count == 0)
                return null;

            var segments = incomingPath.Trim()
                                       .Replace('/', '\\')
                                       .Split('\\', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                return null;

            // Normalize all roots once. Invalid or empty entries become null and are skipped below.
            string?[] rootsFull = localRoots.Select(NormalizeRoot).ToArray();

            // Pass 1: exact anchor match against every root, in spread order.
            for (int i = 0; i < rootsFull.Length; i++)
            {
                var rootFull = rootsFull[i];
                if (rootFull == null) continue;

                // Anchor is the last folder name of this root, e.g. "VJAssets".
                var anchor = new DirectoryInfo(rootFull).Name;

                int anchorIndex = Array.FindLastIndex(segments,
                    s => s.Equals(anchor, StringComparison.OrdinalIgnoreCase));
                if (anchorIndex < 0) continue;

                var relative = Path.Combine(segments[(anchorIndex + 1)..]);
                var candidate = SafeCombine(rootFull, relative);
                if (candidate != null && Exists(candidate))
                {
                    rootIndex = i;
                    return candidate;
                }
            }

            if (!useSuffixProbing)
                return null;

            // Pass 2: suffix probing. Longest tail first; for each tail, every root in order.
            // Minimum 2 segments so a lone file name cannot produce a false positive.
            for (int length = segments.Length; length >= 2; length--)
            {
                var tail = Path.Combine(segments[^length..]);

                for (int i = 0; i < rootsFull.Length; i++)
                {
                    var rootFull = rootsFull[i];
                    if (rootFull == null) continue;

                    var candidate = SafeCombine(rootFull, tail);
                    if (candidate != null && Exists(candidate))
                    {
                        rootIndex = i;
                        return candidate;
                    }
                }
            }

            return null;
        }

        // Returns the full path of a root, or null if the string is empty or not a valid path.
        private static string? NormalizeRoot(string? root)
        {
            if (string.IsNullOrWhiteSpace(root)) return null;
            try { return Path.GetFullPath(root.Trim()); }
            catch (Exception) { return null; }
        }

        // Combines and normalizes, rejects results outside the root (e.g. via "..").
        private static string? SafeCombine(string rootFull, string relative)
        {
            var full = Path.GetFullPath(Path.Combine(rootFull, relative));
            return full.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase) ? full : null;
        }

        private static bool Exists(string path) => File.Exists(path) || Directory.Exists(path);
    }
}
