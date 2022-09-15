// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class ImageSelector : ObservableRecipient, IRecipient<MessageRequestControl>, IAssetSelector, IDropTarget
    {
        public ImageSelector(AssetImage defaultAsset, ImageSelectorModel geometrySelectorModel)
        {
            this.ID = geometrySelectorModel.ID;
            SelectedAsset = defaultAsset;

            IsActive = true;
        }


        public Guid ID { get; set; }


        private IAsset _selectedAsset;
        public IAsset SelectedAsset
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
            ImageSelectorModel assetPathSelectorModel = model as ImageSelectorModel;
            this.ID = assetPathSelectorModel.ID;

            if (this.SelectedAsset == null)
                this.SelectedAsset = new AssetImage();

            if (assetPathSelectorModel.SelectedAsset != null)
                this.SelectedAsset.SetViewModel(assetPathSelectorModel.SelectedAsset);
        }

        public IModel GetModel()
        {
            ImageSelectorModel model = new ImageSelectorModel();
            model.ID = this.ID;
            if (this.SelectedAsset != null)
                model.SelectedAsset = (IAssetModel)this.SelectedAsset.GetModel();
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
                message.Reply(this);
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
