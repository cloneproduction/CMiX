using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace CMiX.UI.Tests.Support
{
    // The data of a drag that holds files and folders, as the file explorer makes it.
    // The storage items come from the storage provider of the window.
    public static class FakeStorageItem
    {
        public static DataObject Drag(TopLevel window, params string[] paths)
        {
            var provider = window.StorageProvider
                ?? throw new InvalidOperationException("The test window has no storage provider.");
            var items = new List<IStorageItem>();
            foreach (var path in paths)
            {
                var uri = new Uri(path);
                IStorageItem? item = Directory.Exists(path)
                    ? provider.TryGetFolderFromPathAsync(uri).GetAwaiter().GetResult()
                    : provider.TryGetFileFromPathAsync(uri).GetAwaiter().GetResult();
                items.Add(item ?? throw new InvalidOperationException("No storage item for " + path));
            }

            var data = new DataObject();
            data.Set(DataFormats.Files, items.ToArray());
            return data;
        }
    }
}
