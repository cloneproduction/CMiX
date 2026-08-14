// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class AssetManager : UserControl
    {
        public AssetManager()
        {
            InitializeComponent();

            // TODO Avalonia: drag drop is wired in the drag and drop phase. The WPF code behind
            // attached an AssetManagerDropHandler to dropBorder, imagesListBox, videosListBox
            // and geometriesListBox when the DataContext became a Core AssetManager.
        }
    }
}
