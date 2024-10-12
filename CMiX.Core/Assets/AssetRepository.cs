// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Assets
{
    public class AssetRepository : ObservableObject
    {
        public AssetRepository()
        {
            Videos = new SortableObservableCollection<Video>();
            Images = new SortableObservableCollection<Image>();
            Geometries = new SortableObservableCollection<Geometry>();
        }


        public void Add(IAsset asset)
        {
            if (asset is Video)
                Videos.Add(asset as Video);

            if (asset is Image)
                Images.Add(asset as Image);

            if(asset is Geometry)
                Geometries.Add(asset as Geometry);
        }



        private SortableObservableCollection<Video> _videos;
        public SortableObservableCollection<Video> Videos
        {
            get => _videos;
            set => SetProperty(ref _videos, value);
        }

        private SortableObservableCollection<Image> _images;
        public SortableObservableCollection<Image> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        private SortableObservableCollection<Geometry> _geometries;
        public SortableObservableCollection<Geometry> Geometries
        {
            get => _geometries;
            set => SetProperty(ref _geometries, value);
        }
    }
}
