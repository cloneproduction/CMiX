// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.


namespace CMiX_ENGINEUtils 
{

    /// <summary>
    /// Maps a path from another machine onto a local root folder.
    /// </summary>
    [ProcessNode]
    public class ResolvePath
    {
        // Cached state so the file system is only touched when inputs change.
        private string? _lastIncoming;
        private string? _lastRoot;
        private string _localPath = "";
        private bool _found;
        private bool _isDirectory;


        public string Update(
            string incomingPath,
            string localRoot,
            bool force,
            out bool found,
            out bool isDirectory)
        {
            bool inputsChanged =
                !string.Equals(incomingPath, _lastIncoming, StringComparison.Ordinal) ||
                !string.Equals(localRoot, _lastRoot, StringComparison.Ordinal);

            if (inputsChanged || force)
            {
                _lastIncoming = incomingPath;
                _lastRoot = localRoot;

                var result = Resolve(incomingPath, localRoot);

                _found = result != null;
                _localPath = result ?? "";
                _isDirectory = _found && Directory.Exists(result);
            }

            found = _found;
            isDirectory = _isDirectory;
            return _localPath;
        }

        private static string? Resolve(string incomingPath, string localRoot)
        {
            if (string.IsNullOrWhiteSpace(incomingPath) || string.IsNullOrWhiteSpace(localRoot))
                return null;

            string rootFull;
            try { rootFull = Path.GetFullPath(localRoot); }
            catch (Exception) { return null; } // Root pin contains an invalid path string.

            // Anchor is the last folder name of the local root, e.g. "VJAssets".
            var anchor = new DirectoryInfo(rootFull).Name;

            var segments = incomingPath.Replace('/', '\\')
                                       .Split('\\', StringSplitOptions.RemoveEmptyEntries);

            // Step 1: exact anchor match, searched from the end so a nested occurrence wins.
            int anchorIndex = Array.FindLastIndex(segments,
                s => s.Equals(anchor, StringComparison.OrdinalIgnoreCase));

            if (anchorIndex >= 0)
            {
                var relative = Path.Combine(segments[(anchorIndex + 1)..]);
                var candidate = SafeCombine(rootFull, relative);
                if (candidate != null && Exists(candidate))
                    return candidate;
            }

            // Step 2: suffix probing, longest tail first, minimum 2 segments.
            for (int length = segments.Length; length >= 2; length--)
            {
                var candidate = SafeCombine(rootFull, Path.Combine(segments[^length..]));
                if (candidate != null && Exists(candidate))
                    return candidate;
            }

            return null;
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
