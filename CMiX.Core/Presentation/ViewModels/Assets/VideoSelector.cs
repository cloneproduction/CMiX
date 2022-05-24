// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using CMiX.Core.Models;
using CMiX.Core.Models.Assets;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class VideoSelector : ObservableRecipient, IRecipient<IMessage>, IControl, IDropTarget
    {
        public VideoSelector(AssetVideo defaultAsset, VideoSelectorModel geometrySelectorModel)
        {
            this.ID = geometrySelectorModel.ID;
            SelectedAsset = defaultAsset;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }

        public VideoSelector(AssetVideo assetVideo, AssetVideo videoSelectorModel)
        {
            this.assetVideo = assetVideo;
            this.videoSelectorModel = videoSelectorModel;
        }

        public Guid ID { get; set; }


        private AssetVideo _selectedAsset;
        private AssetVideo assetVideo;
        private AssetVideo videoSelectorModel;

        public AssetVideo SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                SetProperty(ref _selectedAsset, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAsset(this, value), MessageType.Out);
            }
        }


        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo != null && dropInfo.DragInfo.SourceItem != null)
                dropInfo.Effects = DragDropEffects.Copy;
        }

        public void Drop(IDropInfo dropInfo)
        {
            //SelectedPath = ((IAssets)dropInfo.DragInfo.SourceItem).Path;
        }

        public void SetViewModel(IModel model)
        {
            VideoSelectorModel assetPathSelectorModel = model as VideoSelectorModel;
            this.ID = assetPathSelectorModel.ID;

            if (this.SelectedAsset == null)
                this.SelectedAsset = new AssetVideo();

            if (assetPathSelectorModel.SelectedAsset != null)
                this.SelectedAsset.SetViewModel(assetPathSelectorModel.SelectedAsset);
        }

        public IModel GetModel()
        {
            VideoSelectorModel model = new VideoSelectorModel();
            model.ID = this.ID;
            if (this.SelectedAsset != null)
                model.SelectedAsset = (AssetVideoModel)this.SelectedAsset.GetModel();
            return model;
        }

        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if (message is MessageAsset messageAsset)
            {
                var assetModel = messageAsset.AssetModel;

                if (assetModel is AssetVideoModel videoModel)
                {
                    var asset = new AssetVideo();
                    asset.SetViewModel(videoModel);
                    this.SelectedAsset = asset;
                    return;
                }
            }
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }
    }
}
